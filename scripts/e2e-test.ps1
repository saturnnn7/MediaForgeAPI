# MediaForge End-to-End Test Suite
# Exercises the full user journey across all services via the Gateway (and direct service URLs where needed).
# Requires: all 7 services running locally, infra (Postgres/Redis/RabbitMQ/MinIO) up.
# Usage: .\scripts\e2e-test.ps1 -AudioFilePath "path\to\file.mp3"

param(
    [string]$AudioFilePath = "",
    [string]$VideoFilePath = ""
)

## Configuration
$BaseUrl = "http://localhost:5000"        # Gateway
$IdentityUrl = "http://localhost:5001"    # Identity direct (for admin tasks)
$CatalogUrl = "http://localhost:5005"     # Catalog direct
$LibraryUrl = "http://localhost:5006"     # Library direct

## Helper functions
function Write-Pass($msg) { Write-Host "  [PASS] $msg" -ForegroundColor Green }
function Write-Fail($msg) { Write-Host "  [FAIL] $msg" -ForegroundColor Red }
function Write-Step($msg) { Write-Host "`n>> $msg" -ForegroundColor Cyan }

function Invoke-Api {
    param($Url, $Method = "GET", $Body = $null, $Token = $null, $FilePath = $null)
    $headers = @{}
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }
    if (-not $FilePath) { $headers["Content-Type"] = "application/json" }
    try {
        $params = @{
            Uri = $Url
            Method = $Method
            UseBasicParsing = $true
            Headers = $headers
        }
        if ($FilePath) { $params["InFile"] = $FilePath }
        elseif ($Body) { $params["Body"] = ($Body | ConvertTo-Json -Depth 10) }
        return Invoke-WebRequest @params
    } catch {
        $errorResponse = $_.Exception.Response
        if ($errorResponse) {
            try {
                $stream = $errorResponse.GetResponseStream()
                $reader = New-Object System.IO.StreamReader($stream)
                $errorBody = $reader.ReadToEnd()
                Write-Host "    Error body: $errorBody" -ForegroundColor DarkRed
            } catch {}
        }
        return $errorResponse
    }
}

function Get-TokenRole($token) {
    $parts = $token.Split('.')
    $payload = $parts[1].Replace('-', '+').Replace('_', '/')
    $mod4 = $payload.Length % 4
    if ($mod4) { $payload += '=' * (4 - $mod4) }
    $decoded = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payload))
    $claims = $decoded | ConvertFrom-Json
    return $claims.role
}

function Get-ResponseBody($response) {
    if ($response -is [System.Net.HttpWebResponse]) { return $null }
    return $response.Content | ConvertFrom-Json
}

function Assert-Status($response, $expected, $stepName) {
    $actual = if ($response -is [System.Net.HttpWebResponse]) {
        [int]$response.StatusCode
    } else {
        [int]$response.StatusCode
    }
    if ($actual -eq $expected) {
        Write-Pass "$stepName (HTTP $actual)"
    } else {
        Write-Fail "$stepName (Expected $expected, got $actual)"
    }
}

function Get-Or-Create-User($email, $password, $displayName) {
    # Try login first so the script is idempotent across re-runs
    $r = Invoke-Api "$BaseUrl/api/auth/login" "POST" @{ email = $email; password = $password }
    if ([int]$r.StatusCode -eq 200) {
        Write-Host "  [INFO] User $email already exists, using existing" -ForegroundColor Yellow
        return Get-ResponseBody $r
    }

    $r = Invoke-Api "$BaseUrl/api/auth/register" "POST" @{
        email = $email; password = $password; displayName = $displayName
    }
    if ([int]$r.StatusCode -eq 201) {
        Write-Pass "Register $displayName (HTTP 201)"
    } else {
        Write-Fail "Register $displayName (HTTP $([int]$r.StatusCode))"
    }
    return Get-ResponseBody $r
}

Write-Host "MediaForge E2E Test Suite" -ForegroundColor Yellow
Write-Host "=========================" -ForegroundColor Yellow
Write-Host "  [INFO] Make sure all services were restarted after key regeneration" -ForegroundColor Yellow
$startTime = Get-Date

## PHASE 1: Health checks
Write-Step "Phase 1: Health Checks"

$services = @(
    @{ Name = "Gateway"; Url = "$BaseUrl/health" },
    @{ Name = "Identity"; Url = "$IdentityUrl/health" },
    @{ Name = "Catalog"; Url = "$CatalogUrl/health" },
    @{ Name = "Library"; Url = "$LibraryUrl/health" }
)
foreach ($svc in $services) {
    $r = Invoke-Api $svc.Url
    Assert-Status $r 200 "$($svc.Name) health"
}

## PHASE 2: User Registration
Write-Step "Phase 2: User Registration & Auth"

Get-Or-Create-User "admin@e2e-test.dev" "AdminPass1!" "E2E Admin" | Out-Null
Get-Or-Create-User "creator@e2e-test.dev" "CreatorPass1!" "E2E Creator" | Out-Null
Get-Or-Create-User "listener@e2e-test.dev" "ListenerPass1!" "E2E Listener" | Out-Null

# Set admin/creator role via direct DB (manual step - no admin-role endpoint exists yet)
Write-Host "  [INFO] Set admin role: docker exec -it mf-postgres psql -U mediaforge -d identity -c `"UPDATE users SET role='admin' WHERE email='admin@e2e-test.dev';`"" -ForegroundColor Yellow
Write-Host "  [INFO] Set creator role: docker exec -it mf-postgres psql -U mediaforge -d identity -c `"UPDATE users SET role='creator' WHERE email='creator@e2e-test.dev';`"" -ForegroundColor Yellow
Write-Host "  Press ENTER after setting roles..." -ForegroundColor Yellow
Read-Host

# Verify emails via direct DB (no SMTP in this script)
Write-Host "  [INFO] Verify emails: docker exec -it mf-postgres psql -U mediaforge -d identity -c `"UPDATE users SET is_email_verified=true;`"" -ForegroundColor Yellow
Write-Host "  Press ENTER after verifying emails..." -ForegroundColor Yellow
Read-Host

## PHASE 3: Login & Tokens
Write-Step "Phase 3: Login"

$r = Invoke-Api "$BaseUrl/api/auth/login" "POST" @{ email = "admin@e2e-test.dev"; password = "AdminPass1!" }
Assert-Status $r 200 "Admin login"
$adminToken = (Get-ResponseBody $r).accessToken

$r = Invoke-Api "$BaseUrl/api/auth/login" "POST" @{ email = "creator@e2e-test.dev"; password = "CreatorPass1!" }
Assert-Status $r 200 "Creator login"
$creatorToken = (Get-ResponseBody $r).accessToken
$creatorRole = Get-TokenRole $creatorToken
Write-Host "  [INFO] Creator token role: $creatorRole" -ForegroundColor Yellow

$r = Invoke-Api "$BaseUrl/api/auth/login" "POST" @{ email = "listener@e2e-test.dev"; password = "ListenerPass1!" }
Assert-Status $r 200 "Listener login"
$listenerToken = (Get-ResponseBody $r).accessToken

# Resolve user IDs from their profile (registration response may be unavailable if the user already existed)
$r = Invoke-Api "$BaseUrl/api/auth/profile" "GET" $null $adminToken
$adminId = (Get-ResponseBody $r).id

$r = Invoke-Api "$BaseUrl/api/auth/profile" "GET" $null $creatorToken
$creatorId = (Get-ResponseBody $r).id

$r = Invoke-Api "$BaseUrl/api/auth/profile" "GET" $null $listenerToken
$listenerId = (Get-ResponseBody $r).id

## PHASE 4: Catalog setup
Write-Step "Phase 4: Catalog - Genre, Person, Series"

# Create Genre (dedicated slug to avoid clashing with prior runs)
$r = Invoke-Api "$BaseUrl/api/genres" "POST" @{ name = "Fantasy"; slug = "fantasy-e2e"; description = "Fantasy genre" } $adminToken
if ([int]$r.StatusCode -eq 409) {
    # No lookup-by-slug endpoint exists - list all genres and find it by slug
    Write-Host "  [INFO] Genre fantasy-e2e already exists, fetching existing" -ForegroundColor Yellow
    $r = Invoke-Api "$BaseUrl/api/genres" "GET" $null $adminToken
    Assert-Status $r 200 "List genres"
    $existing = (Get-ResponseBody $r) | Where-Object { $_.slug -eq "fantasy-e2e" }
    if ($existing) {
        $genreId = $existing.id
        Write-Host "  [INFO] Using existing genre: $genreId" -ForegroundColor Yellow
    }
} else {
    Assert-Status $r 201 "Create genre"
    $genreId = (Get-ResponseBody $r).id
}

# Create Person (Author)
$r = Invoke-Api "$BaseUrl/api/persons" "POST" @{ name = "Test Author"; bio = "A great author" } $adminToken
Assert-Status $r 201 "Create person (author)"
$personId = (Get-ResponseBody $r).id

## PHASE 5: Creator Channel
Write-Step "Phase 5: Creator Channel"

# Creator must already have the creator role (set via DB above)
$r = Invoke-Api "$BaseUrl/api/channels" "POST" @{ name = "E2E Creator Channel" } $creatorToken
if ([int]$r.StatusCode -eq 201) {
    Write-Pass "Create channel (HTTP 201)"
    $channelId = (Get-ResponseBody $r).id
} elseif ([int]$r.StatusCode -eq 400) {
    # Already has a channel (expected on re-runs) - fetch the existing one
    $r2 = Invoke-Api "$BaseUrl/api/channels/me" "GET" $null $creatorToken
    if ([int]$r2.StatusCode -eq 200) {
        $channelId = (Get-ResponseBody $r2).id
        Write-Host "  [INFO] Using existing channel $channelId" -ForegroundColor Yellow
    }
} else {
    Write-Fail "Create channel (Expected 201, got $([int]$r.StatusCode))"
}

## PHASE 6: Work Request & Approval
Write-Step "Phase 6: Work Request Workflow"

if (-not $channelId) {
    Write-Host "  [SKIP] No channel available - skipping work request workflow" -ForegroundColor Yellow
} else {
    # Creator submits work request
    $r = Invoke-Api "$BaseUrl/api/work-requests" "POST" @{
        workType = 0
        title = "E2E Test Audiobook"
        authorNames = "Test Author"
        description = "A test audiobook for E2E testing"
        language = "en"
    } $creatorToken
    Assert-Status $r 201 "Submit work request"
    $requestId = (Get-ResponseBody $r).id

    # Admin approves
    $r = Invoke-Api "$BaseUrl/api/work-requests/$requestId/approve" "POST" @{ channelId = $channelId } $adminToken
    Assert-Status $r 200 "Admin approves work request"
    $workId = (Get-ResponseBody $r).resultingWorkId

    if ($workId) {
        # Verify work was created
        $r = Invoke-Api "$BaseUrl/api/works/$workId"
        Assert-Status $r 200 "Work created from request"
    }
}

## PHASE 7: Edition & Parts
Write-Step "Phase 7: Edition & Parts"

if (-not $workId) {
    Write-Host "  [SKIP] Work not created - skipping edition/parts" -ForegroundColor Yellow
} else {
    # Create Edition
    $r = Invoke-Api "$BaseUrl/api/editions" "POST" @{
        workId = $workId
        narratorTeamName = "E2E Narrator Team"
        language = "en"
    } $creatorToken
    Assert-Status $r 201 "Create edition"
    $editionId = (Get-ResponseBody $r).id

    # Create Part
    $r = Invoke-Api "$BaseUrl/api/parts" "POST" @{
        editionId = $editionId
        title = "Chapter 1 - The Beginning"
        orderMajor = 1
        orderMinor = 0
        partType = 0
    } $creatorToken
    Assert-Status $r 201 "Create part"
    $partId = (Get-ResponseBody $r).id
}

## PHASE 8: Media Upload
Write-Step "Phase 8: Media Upload & Processing"

if (-not $partId) {
    Write-Host "  [SKIP] Part not created - skipping media upload" -ForegroundColor Yellow
} elseif ($AudioFilePath -and (Test-Path $AudioFilePath)) {
    # Request upload URL
    $fileName = Split-Path $AudioFilePath -Leaf
    $r = Invoke-Api "$BaseUrl/api/media/upload-url" "POST" @{
        fileName = $fileName
        contentType = "audio/mpeg"
        fileSizeBytes = (Get-Item $AudioFilePath).Length
    } $creatorToken
    Assert-Status $r 201 "Request upload URL"
    $uploadData = Get-ResponseBody $r
    $assetId = $uploadData.assetId
    $uploadUrl = $uploadData.uploadUrl

    # Upload to MinIO via HttpClient. The presigned URL no longer bakes Content-Type into
    # its signature (see StorageService.GenerateUploadUrlAsync), so no header matching is needed.
    if (-not $uploadUrl) {
        Write-Fail "Upload URL is empty - skipping upload"
    } else {
        Add-Type -AssemblyName System.Net.Http

        $handler = New-Object System.Net.Http.HttpClientHandler
        $httpClient = New-Object System.Net.Http.HttpClient($handler)
        $httpClient.DefaultRequestHeaders.Clear()

        $fileBytes = [System.IO.File]::ReadAllBytes($AudioFilePath)
        # Comma prevents New-Object from unpacking the byte[] into individual constructor args
        $content = New-Object System.Net.Http.ByteArrayContent(,$fileBytes)

        try {
            $uploadResult = $httpClient.PutAsync($uploadUrl, $content).GetAwaiter().GetResult()
            $statusCode = [int]$uploadResult.StatusCode
            if ($statusCode -eq 200) {
                Write-Pass "Upload file to MinIO (HTTP 200)"
                $uploadSuccess = $true
            } else {
                $body = $uploadResult.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                Write-Fail "Upload file to MinIO (Expected 200, got $statusCode): $body"
                $uploadSuccess = $false
            }
        } finally {
            $httpClient.Dispose()
        }
    }

    # Confirm upload
    $r = Invoke-Api "$BaseUrl/api/media/$assetId/confirm-upload" "POST" $null $creatorToken
    Assert-Status $r 202 "Confirm upload"

    Write-Host "  [INFO] Waiting for processing (30 seconds)..." -ForegroundColor Yellow
    Start-Sleep -Seconds 30

    # Check asset status
    $r = Invoke-Api "$BaseUrl/api/media/$assetId" "GET" $null $creatorToken
    $status = (Get-ResponseBody $r).status
    if ($status -eq "Completed") {
        Write-Pass "Asset processing completed"
    } else {
        Write-Fail "Asset processing - status: $status (may need more time)"
    }

    # Link asset to part
    $r = Invoke-Api "$BaseUrl/api/media/$assetId/part" "PATCH" @{ partId = $partId } $creatorToken
    Assert-Status $r 200 "Link asset to part"

    $r = Invoke-Api "$BaseUrl/api/parts/$partId/assets" "POST" @{
        mediaAssetId = $assetId; sequenceOrder = 1
    } $creatorToken
    Assert-Status $r 201 "Link asset in catalog"
} else {
    Write-Host "  [SKIP] No audio file provided - pass -AudioFilePath to test upload" -ForegroundColor Yellow
}

## PHASE 9: Publish Work
Write-Step "Phase 9: Publish Work"

if (-not $workId) {
    Write-Host "  [SKIP] Work not created - skipping publish" -ForegroundColor Yellow
} else {
    $r = Invoke-Api "$BaseUrl/api/works/$workId/publish" "POST" $null $adminToken
    Assert-Status $r 200 "Publish work"

    if ($partId) {
        $r = Invoke-Api "$BaseUrl/api/parts/$partId/publish" "POST" $null $creatorToken
        Assert-Status $r 200 "Publish part"
    }
}

## PHASE 10: Library & Reviews
Write-Step "Phase 10: Listener - Library & Reviews"

if (-not $workId) {
    Write-Host "  [SKIP] Work not created - skipping library/reviews" -ForegroundColor Yellow
} else {
    # Add to library
    $r = Invoke-Api "$BaseUrl/api/library" "POST" @{
        workId = $workId; status = 0; privacy = 0
    } $listenerToken
    Assert-Status $r 201 "Add work to library"

    # Rate the work
    $r = Invoke-Api "$BaseUrl/api/library/$workId/rating" "PUT" @{ rating = 8 } $listenerToken
    Assert-Status $r 200 "Rate work (8/10)"

    # Write review
    $r = Invoke-Api "$BaseUrl/api/reviews" "POST" @{
        workId = $workId
        text = "Excellent audiobook! The narrator team did a fantastic job. ||The ending was unexpected||"
        containsSpoiler = $true
    } $listenerToken
    Assert-Status $r 201 "Write review"
    $reviewId = (Get-ResponseBody $r).id

    if ($reviewId) {
        # Add comment
        $r = Invoke-Api "$BaseUrl/api/reviews/$reviewId/comments" "POST" @{
            text = "I totally agree with this review!"
            containsSpoiler = $false
        } $listenerToken
        Assert-Status $r 201 "Add comment to review"
    }
}

## PHASE 11: Social Features
Write-Step "Phase 11: Social Features"

# Listener sends friend request to creator
$r = Invoke-Api "$BaseUrl/api/friends/request/$creatorId" "POST" $null $listenerToken
Assert-Status $r 201 "Send friend request"
$friendshipId = (Get-ResponseBody $r).id

if ($friendshipId) {
    # Creator accepts
    $r = Invoke-Api "$BaseUrl/api/friends/$friendshipId/accept" "POST" $null $creatorToken
    Assert-Status $r 200 "Accept friend request"
}

# Check listener notification
$r = Invoke-Api "$BaseUrl/api/notifications?page=1&pageSize=20" "GET" $null $listenerToken
Assert-Status $r 200 "Get notifications"
$notifCount = (Get-ResponseBody $r).Count
if ($notifCount -gt 0) { Write-Pass "Notification received ($notifCount)" }
else { Write-Fail "No notifications found" }

# Follow author
$r = Invoke-Api "$BaseUrl/api/authors/$personId/follow" "POST" $null $listenerToken
Assert-Status $r 201 "Follow author"

## PHASE 12: Search
Write-Step "Phase 12: Search"

Write-Host "  [INFO] Search requires Elasticsearch: docker compose -f infra/docker-compose.yml --profile search up -d" -ForegroundColor Yellow

Start-Sleep -Seconds 2
$r = Invoke-Api "$BaseUrl/api/search/works?q=E2E" "GET" $null $listenerToken
Assert-Status $r 200 "Search works"

## Summary
Write-Host "`n=========================" -ForegroundColor Yellow
$elapsed = (Get-Date) - $startTime
Write-Host "E2E Test Complete in $($elapsed.TotalSeconds.ToString('F1'))s" -ForegroundColor Yellow

Write-Host "`nTest Data Created:" -ForegroundColor Cyan
Write-Host "  workId:    $workId"
Write-Host "  editionId: $editionId"
Write-Host "  partId:    $partId"
if ($assetId) { Write-Host "  assetId:   $assetId" }
Write-Host "  channelId: $channelId"
Write-Host "  genreId:   $genreId"
Write-Host "  personId:  $personId"
