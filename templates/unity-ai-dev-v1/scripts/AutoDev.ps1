param([switch]$NoOpen)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$ProjectRoot = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$ConfigPath = Join-Path $ProjectRoot 'ai-dev.config.json'
if (-not (Test-Path $ConfigPath)) { throw "Missing $ConfigPath" }
$config = Get-Content $ConfigPath -Raw | ConvertFrom-Json

function Stop-Fail([string]$Message) {
    Write-Host "[AutoDev] STOP: $Message" -ForegroundColor Red
    exit 1
}

function Find-Unity {
    if ($config.unityExecutable -and $config.unityExecutable -ne 'AUTO' -and (Test-Path $config.unityExecutable)) {
        return $config.unityExecutable
    }
    $versionFile = Join-Path $ProjectRoot 'ProjectSettings\ProjectVersion.txt'
    if (-not (Test-Path $versionFile)) { return $null }
    $line = Get-Content $versionFile | Where-Object { $_ -match '^m_EditorVersion:' } | Select-Object -First 1
    if (-not $line) { return $null }
    $version = ($line -split ':',2)[1].Trim()
    $candidates = @(
        "C:\Program Files\Unity\Hub\Editor\$version\Editor\Unity.exe",
        "D:\DL\unity\$version\Editor\Unity.exe",
        "D:\Unity\Hub\Editor\$version\Editor\Unity.exe"
    )
    return $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
}

Set-Location $ProjectRoot
if (-not (Get-Command git -ErrorAction SilentlyContinue)) { Stop-Fail 'Git was not found.' }
if ($config.requireCleanWorkingTree -and (git status --porcelain)) {
    Stop-Fail 'Local changes exist. Commit or stash them first.'
}

$branch = if ($config.mainBranch) { [string]$config.mainBranch } else { 'main' }
& git pull --ff-only origin $branch
if ($LASTEXITCODE -ne 0) { Stop-Fail 'git pull --ff-only failed.' }

$unityExe = Find-Unity
if (-not $unityExe) { Stop-Fail 'Unity Editor executable was not found.' }
if (-not $config.smokeExecuteMethod -or $config.smokeExecuteMethod -like 'REPLACE_*') {
    Stop-Fail 'Set smokeExecuteMethod in ai-dev.config.json before using AutoDev.'
}

$logRoot = Join-Path $ProjectRoot 'Logs\AutoDev'
New-Item -ItemType Directory -Force -Path $logRoot | Out-Null
$maxAttempts = [int]$config.maxAttempts
$sameErrorStop = [int]$config.sameErrorStop
$lastSignature = $null
$sameCount = 0
$passed = $false

for ($attempt = 1; $attempt -le $maxAttempts; $attempt++) {
    $log = Join-Path $logRoot ("unity-smoke-{0}-attempt{1}.log" -f (Get-Date -Format 'yyyyMMdd-HHmmss'), $attempt)
    $args = @('-batchmode','-nographics','-projectPath',$ProjectRoot,'-executeMethod',[string]$config.smokeExecuteMethod,'-logFile',$log)
    $proc = Start-Process -FilePath $unityExe -ArgumentList $args -PassThru -Wait
    if ($proc.ExitCode -eq 0) { $passed = $true; break }

    $signature = if (Test-Path $log) {
        ((Get-Content $log | Where-Object { $_ -match 'error CS\d+|Exception|Compilation failed|Scripts have compiler errors' } | Select-Object -Last 8) -join ' | ')
    } else { "exit:$($proc.ExitCode):no-log" }

    if ($signature -eq $lastSignature) { $sameCount++ } else { $sameCount = 1; $lastSignature = $signature }
    Write-Host "[AutoDev] FAIL: $signature"
    if ($sameCount -ge $sameErrorStop) { Stop-Fail 'The same Unity error repeated. Stopping safely.' }
}

if (-not $passed) { Stop-Fail "Unity validation failed after $maxAttempts attempts." }

$commit = (git rev-parse HEAD).Trim()
Set-Content -Path (Join-Path $ProjectRoot '.git\autodev-last-good') -Value $commit -Encoding ASCII
& (Join-Path $ProjectRoot 'scripts\update-project-state.ps1') -StatePath (Join-Path $ProjectRoot 'project-memory\PROJECT_STATE.md') -BuildOutcome 'PASS' -TestOutcome 'PASS' -CommitSha $commit
Write-Host "[AutoDev] PASS. Last-good checkpoint: $commit"

if (-not $NoOpen -and $config.openUnityAfterPass) {
    Start-Process -FilePath $unityExe -ArgumentList @('-projectPath',$ProjectRoot) | Out-Null
}
