# Commit-msg hook to validate conventional commit messages (PowerShell)

$commitMsgFile = $args[0]

if (-not $commitMsgFile) {
    Write-Host "❌ Error: Commit message file path not provided" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $commitMsgFile)) {
    Write-Host "❌ Error: Commit message file not found: $commitMsgFile" -ForegroundColor Red
    exit 1
}

$commitMsg = (Get-Content -Path $commitMsgFile -Raw).Trim()

if ([string]::IsNullOrWhiteSpace($commitMsg)) {
    Write-Host "❌ Error: Commit message is empty" -ForegroundColor Red
    exit 1
}

$conventionalPattern = '^(feat|fix|docs|style|refactor|test|chore|perf|ci|build|revert)(\(.+\))?: .+'
$mergePattern = '^Merge .+'
$revertPattern = '^Revert .+'

if ($commitMsg -match $conventionalPattern) {
    exit 0
}

if ($commitMsg -match $mergePattern) {
    exit 0
}

if ($commitMsg -match $revertPattern) {
    exit 0
}

Write-Host "❌ Invalid commit message format!" -ForegroundColor Red
Write-Host ""
Write-Host "Commit messages must follow the Conventional Commits specification:"
Write-Host "  type(scope): subject"
Write-Host ""
Write-Host "Allowed types: feat, fix, docs, style, refactor, test, chore, perf, ci, build, revert"
Write-Host ""
Write-Host "Examples:"
Write-Host "  feat: add new feature"
Write-Host "  fix(core): resolve bug in parsing"
Write-Host "  docs: update README"
Write-Host "  chore: update dependencies"
Write-Host ""
Write-Host "Your message:"
Write-Host "  $commitMsg"
Write-Host ""
exit 1