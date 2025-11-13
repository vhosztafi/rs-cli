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
        --logger:"console;verbosity=normal"

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Tests failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }

    Write-Host "`nTests completed successfully!" -ForegroundColor Green
    Write-Host "Coverage data saved to TestResults directory" -ForegroundColor Green
}
finally {
    Pop-Location
}