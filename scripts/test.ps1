#!/usr/bin/env pwsh
# Test script for Windows (PowerShell)
# Runs all tests with code coverage collection

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
Push-Location $repoRoot

try {
    Write-Host "Running tests with code coverage..." -ForegroundColor Cyan

    dotnet test `
        --collect:"XPlat Code Coverage" `
        --results-directory:"TestResults" `
        --logger:"console;verbosity=normal" `
        -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Tests failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }

    Write-Host "`nTests completed successfully!" -ForegroundColor Green
    Write-Host "Coverage data (Cobertura XML format) saved to TestResults directory" -ForegroundColor Green
    Write-Host "To generate HTML reports, run: .\scripts\generate-coverage-report.ps1" -ForegroundColor Yellow
}
finally {
    Pop-Location
}