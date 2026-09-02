param(
    [Parameter(Mandatory = $true)][string]$StatePath,
    [Parameter(Mandatory = $true)][string]$BuildOutcome,
    [Parameter(Mandatory = $true)][string]$TestOutcome,
    [string]$RunId = 'LOCAL',
    [string]$RunUrl = 'N/A',
    [string]$CommitSha = 'N/A',
    [string]$UpdatedAtUtc = ''
)

$ErrorActionPreference = 'Stop'
if (-not (Test-Path $StatePath)) { throw "Missing $StatePath" }
if ([string]::IsNullOrWhiteSpace($UpdatedAtUtc)) {
    $UpdatedAtUtc = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
}

$machineBlock = @"
<!-- CI_STATE_START -->
Build: $BuildOutcome
Tests: $TestOutcome
Run ID: $RunId
Run URL: $RunUrl
Commit SHA: $CommitSha
Updated At UTC: $UpdatedAtUtc
<!-- CI_STATE_END -->
"@

$content = Get-Content $StatePath -Raw
$pattern = '(?s)<!-- CI_STATE_START -->.*?<!-- CI_STATE_END -->'
if ($content -notmatch $pattern) { throw 'CI state markers are missing from PROJECT_STATE.md' }
Set-Content -Path $StatePath -Value ([regex]::Replace($content, $pattern, $machineBlock.Trim())) -Encoding utf8
