#!/usr/bin/env pwsh
# Coverage report generation script for Windows (PowerShell)
# Generates HTML coverage reports from Cobertura XML files

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptDir
Push-Location $repoRoot

try {
    Write-Host "Generating code coverage HTML reports..." -ForegroundColor Cyan

    $reportGenerator = dotnet tool list -g | Select-String "reportgenerator"
    $toolInstalled = $false
    
    if (-not $reportGenerator) {
        Write-Host "Installing reportgenerator tool..." -ForegroundColor Yellow
        dotnet tool install -g dotnet-reportgenerator-globaltool
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to install reportgenerator tool!" -ForegroundColor Red
            exit 1
        }
        $toolInstalled = $true
    }
    
    $userProfile = $env:USERPROFILE
    $dotnetToolsPath = Join-Path $userProfile ".dotnet\tools"
    
    if ($toolInstalled -or (-not (Get-Command "reportgenerator" -ErrorAction SilentlyContinue))) {
        if (Test-Path $dotnetToolsPath) {
            if ($env:Path -notlike "*$dotnetToolsPath*") {
                $env:Path = "$dotnetToolsPath;$env:Path"
            }
        }
    }
    
    $reportGeneratorCmd = Get-Command "reportgenerator" -ErrorAction SilentlyContinue
    if (-not $reportGeneratorCmd) {
        $reportGeneratorExe = Join-Path $dotnetToolsPath "reportgenerator.exe"
        if (Test-Path $reportGeneratorExe) {
            $reportGeneratorCmd = $reportGeneratorExe
        } else {
            Write-Host "Could not find reportgenerator tool. Please restart your PowerShell session and try again." -ForegroundColor Red
            exit 1
        }
    } else {
        $reportGeneratorCmd = $reportGeneratorCmd.Source
    }

    $coverageFiles = Get-ChildItem -Path "TestResults" -Recurse -Filter "coverage.cobertura.xml" -ErrorAction SilentlyContinue
    
    if ($coverageFiles.Count -eq 0) {
        Write-Host "No coverage files found in TestResults directory." -ForegroundColor Yellow
        Write-Host "Please run tests first using: .\scripts\test.ps1" -ForegroundColor Yellow
        exit 1
    }

    Write-Host "Found $($coverageFiles.Count) coverage file(s)" -ForegroundColor Green

    $outputDir = "TestResults\coverage\html"
    if (Test-Path $outputDir) {
        Remove-Item -Path $outputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

    $coverageFilePaths = ($coverageFiles | ForEach-Object { $_.FullName }) -join ";"
    
    & $reportGeneratorCmd `
        -reports:"$coverageFilePaths" `
        -targetdir:"$outputDir" `
        -reporttypes:"Html"

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to generate coverage reports!" -ForegroundColor Red
        exit 1
    }

    $htmlReportPath = Join-Path (Join-Path $repoRoot $outputDir) "index.html"
    Write-Host "`nCoverage reports generated successfully!" -ForegroundColor Green
    Write-Host "HTML report location: $htmlReportPath" -ForegroundColor Green
    Write-Host "Open the report in your browser to view coverage details." -ForegroundColor Yellow
} finally {
    Pop-Location
}