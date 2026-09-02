param(
    [Parameter(Mandatory = $true)]
    [string]$StatePath,

    [Parameter(Mandatory = $true)]
    [string]$RestoreOutcome,

    [Parameter(Mandatory = $true)]
    [string]$BuildOutcome,

    [Parameter(Mandatory = $true)]
    [string]$TestOutcome,

    [Parameter(Mandatory = $true)]
    [string]$RunId,

    [Parameter(Mandatory = $true)]
    [string]$RunUrl,

    [Parameter(Mandatory = $true)]
    [string]$CommitSha,

    [string]$UpdatedAtUtc = ''
)

if (-not (Test-Path $StatePath)) {
    throw "Missing $StatePath"
}

$build = if ($RestoreOutcome -eq 'success' -and $BuildOutcome -eq 'success') { 'PASS' } else { 'FAIL' }
$tests = if ($build -ne 'PASS') { 'NOT_RUN' } elseif ($TestOutcome -eq 'success') { 'PASS' } else { 'FAIL' }

if ([string]::IsNullOrWhiteSpace($UpdatedAtUtc)) {
    $UpdatedAtUtc = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
}

$machineBlock = @"
<!-- CI_STATE_START -->
Build: $build
Tests: $tests
Run ID: $RunId
Run URL: $RunUrl
Commit SHA: $CommitSha
Updated At UTC: $UpdatedAtUtc
<!-- CI_STATE_END -->
"@

$content = Get-Content $StatePath -Raw
$pattern = '(?s)<!-- CI_STATE_START -->.*?<!-- CI_STATE_END -->'
if ($content -notmatch $pattern) {
    throw 'CI state markers are missing from PROJECT_STATE.md'
}

$newContent = [regex]::Replace($content, $pattern, $machineBlock.Trim())
Set-Content -Path $StatePath -Value $newContent -Encoding utf8

[PSCustomObject]@{
    Build = $build
    Tests = $tests
    StatePath = $StatePath
}
