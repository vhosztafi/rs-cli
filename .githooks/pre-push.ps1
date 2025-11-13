# Pre-push hook to run unit tests before pushing (PowerShell)

Write-Host "Running unit tests before push..." -ForegroundColor Cyan
Write-Host ""

Write-Host "Building project..." -ForegroundColor Cyan
$buildResult = dotnet build --configuration Release --no-restore 2>&1
if ($LASTEXITCODE -ne 0) {
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "❌ Build failed. Push aborted." -ForegroundColor Red
        Write-Host "Please fix build errors before pushing." -ForegroundColor Red
        exit 1
    }
}

# Run tests excluding integration tests
$testResult = dotnet test --filter "FullyQualifiedName!~Integration.Tests" --no-build --configuration Release

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ All unit tests passed!" -ForegroundColor Green
    exit 0
} else {
    Write-Host ""
    Write-Host "❌ Unit tests failed. Push aborted." -ForegroundColor Red
    Write-Host "Please fix the failing tests before pushing." -ForegroundColor Red
    exit 1
}

