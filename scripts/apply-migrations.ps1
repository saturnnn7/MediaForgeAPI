Write-Host "Applying all MediaForge migrations..." -ForegroundColor Cyan

$migrations = @(
    @{ Project = "src/Services/Identity/Infrastructure"; Startup = "src/Services/Identity/API"; Context = "IdentityDbContext" },
    @{ Project = "src/Services/Identity/Infrastructure"; Startup = "src/Services/Identity/API"; Context = "ConfigurationDbContext" },
    @{ Project = "src/Services/Identity/Infrastructure"; Startup = "src/Services/Identity/API"; Context = "PersistedGrantDbContext" },
    @{ Project = "src/Services/Media/Infrastructure"; Startup = "src/Services/Media/API"; Context = "MediaDbContext" },
    @{ Project = "src/Services/Catalog/Infrastructure"; Startup = "src/Services/Catalog/API"; Context = "CatalogDbContext" },
    @{ Project = "src/Services/Library/Infrastructure"; Startup = "src/Services/Library/API"; Context = "LibraryDbContext" }
)

foreach ($m in $migrations) {
    Write-Host "  Applying $($m.Context)..." -ForegroundColor Yellow
    dotnet ef database update `
        --project $m.Project `
        --startup-project $m.Startup `
        --context $m.Context
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  FAILED: $($m.Context)" -ForegroundColor Red
        exit 1
    }
    Write-Host "  Done: $($m.Context)" -ForegroundColor Green
}

Write-Host "All migrations applied successfully." -ForegroundColor Cyan
