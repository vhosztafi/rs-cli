# Install git hooks by configuring git to use the .githooks directory (PowerShell)

Write-Host "Installing git hooks..." -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path ".githooks")) {
    Write-Host "❌ Error: .githooks directory not found!" -ForegroundColor Red
    Write-Host "Please run this script from the repository root." -ForegroundColor Red
    exit 1
}

$commitMsgWrapper = @'
#!/bin/sh
# Wrapper to detect shell and call appropriate commit-msg hook
if [ -n "$PSVersionTable" ] || [ -n "$POWERSHELL_DISTRIBUTION_CHANNEL" ]; then
    pwsh -File .githooks/commit-msg.ps1 "$@"
else
    .githooks/commit-msg "$@"
fi
'@

$prePushWrapper = @'
#!/bin/sh
# Wrapper to detect shell and call appropriate pre-push hook
if [ -n "$PSVersionTable" ] || [ -n "$POWERSHELL_DISTRIBUTION_CHANNEL" ]; then
    pwsh -File .githooks/pre-push.ps1
else
    .githooks/pre-push
fi
'@

$result = git config core.hooksPath .githooks

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Git hooks installed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Hooks configured:"
    Write-Host "  - commit-msg: Validates conventional commit messages"
    Write-Host "  - pre-push: Runs unit tests before pushing"
    Write-Host ""
    Write-Host "Note: Hooks will run in Git Bash environment on Windows."
    Write-Host "To uninstall, run: git config --unset core.hooksPath"
} else {
    Write-Host "❌ Failed to install git hooks" -ForegroundColor Red
    exit 1
}