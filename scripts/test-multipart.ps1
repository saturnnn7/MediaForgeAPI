<#
.SYNOPSIS
    Manually exercises the S3 multipart upload flow against Media.API (via Gateway).
.PARAMETER Token
    A valid JWT access token (no "Bearer " prefix), e.g. copied from the Bruno {{accessToken}} env var.
.PARAMETER FilePath
    Path to the local file to upload.
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$Token,

    [Parameter(Mandatory = $true)]
    [string]$FilePath
)

$ErrorActionPreference = "Stop"

$BaseUrl = "http://localhost:5000"
$ChunkSize = 5 * 1024 * 1024

if (-not (Test-Path $FilePath)) {
    throw "File not found: $FilePath"
}

$fileInfo = Get-Item $FilePath
$fileSizeBytes = $fileInfo.Length
$fileName = $fileInfo.Name

$contentTypeMap = @{
    ".mp4" = "video/mp4"
    ".mov" = "video/quicktime"
    ".mp3" = "audio/mpeg"
    ".wav" = "audio/wav"
    ".ogg" = "audio/ogg"
}
$contentType = $contentTypeMap[$fileInfo.Extension.ToLowerInvariant()]
if (-not $contentType) {
    throw "Unsupported file extension '$($fileInfo.Extension)'. Allowed: $($contentTypeMap.Keys -join ', ')"
}

$headers = @{ Authorization = "Bearer $Token" }

Write-Host "File: $fileName ($fileSizeBytes bytes, $contentType)"
Write-Host "Initiating multipart upload..."

$initiateBody = @{
    fileName      = $fileName
    contentType   = $contentType
    fileSizeBytes = $fileSizeBytes
} | ConvertTo-Json

$initiateResponse = Invoke-RestMethod -Uri "$BaseUrl/api/media/multipart/initiate" `
    -Method Post -Headers $headers -ContentType "application/json" -Body $initiateBody

$assetId = $initiateResponse.assetId
$uploadId = $initiateResponse.uploadId

Write-Host "Asset ID:  $assetId"
Write-Host "Upload ID: $uploadId"

$partCount = [Math]::Ceiling($fileSizeBytes / $ChunkSize)
Write-Host "Uploading $partCount part(s) of up to $($ChunkSize / 1MB)MB each..."

$parts = New-Object System.Collections.Generic.List[hashtable]

$stream = [System.IO.File]::OpenRead($FilePath)
try {
    for ($partNumber = 1; $partNumber -le $partCount; $partNumber++) {
        $remaining = $fileSizeBytes - $stream.Position
        $currentChunkSize = [Math]::Min($ChunkSize, $remaining)

        $buffer = New-Object byte[] $currentChunkSize
        $totalRead = 0
        while ($totalRead -lt $currentChunkSize) {
            $read = $stream.Read($buffer, $totalRead, $currentChunkSize - $totalRead)
            if ($read -eq 0) {
                throw "Unexpected end of file while reading part $partNumber."
            }
            $totalRead += $read
        }

        $partUrlBody = @{
            assetId    = $assetId
            uploadId   = $uploadId
            partNumber = $partNumber
        } | ConvertTo-Json

        $partUrlResponse = Invoke-RestMethod -Uri "$BaseUrl/api/media/multipart/part-url" `
            -Method Post -Headers $headers -ContentType "application/json" -Body $partUrlBody

        $signedUrl = $partUrlResponse.url

        Write-Host "  Part $partNumber/$partCount ($totalRead bytes) -> uploading..."

        $putResponse = Invoke-WebRequest -Uri $signedUrl -Method Put -Body $buffer -UseBasicParsing

        $eTag = $putResponse.Headers['ETag']
        if ($eTag -is [array]) {
            $eTag = $eTag[0]
        }
        if (-not $eTag) {
            throw "Part $partNumber upload did not return an ETag header."
        }

        Write-Host "  Part $partNumber ETag: $eTag"

        $parts.Add(@{ partNumber = $partNumber; eTag = $eTag })
    }
}
finally {
    $stream.Dispose()
}

Write-Host "Completing multipart upload..."

$completeBody = @{
    assetId  = $assetId
    uploadId = $uploadId
    parts    = $parts
} | ConvertTo-Json -Depth 5

$completeResponse = Invoke-RestMethod -Uri "$BaseUrl/api/media/multipart/complete" `
    -Method Post -Headers $headers -ContentType "application/json" -Body $completeBody

Write-Host ""
Write-Host "Upload complete."
Write-Host "Asset ID: $($completeResponse.assetId)"
Write-Host "Status:   $($completeResponse.status)"
