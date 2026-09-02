param(
    [Parameter(Mandatory = $true)]
    [string]$TargetProject,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'
$TemplateRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$TargetProject = (Resolve-Path $TargetProject).Path

if (-not (Test-Path (Join-Path $TargetProject 'Assets')) -or
    -not (Test-Path (Join-Path $TargetProject 'ProjectSettings'))) {
    throw "Target does not look like a Unity project: $TargetProject"
}

$items = @(
    'project-memory',
    'scripts',
    'ai-dev.config.json'
)

foreach ($item in $items) {
    $source = Join-Path $TemplateRoot $item
    if (-not (Test-Path $source)) { continue }
    $destination = Join-Path $TargetProject $item

    if ((Test-Path $destination) -and -not $Force) {
        Write-Host "SKIP existing: $destination"
        continue
    }

    Copy-Item -Path $source -Destination $destination -Recurse -Force:$Force
    Write-Host "COPIED: $item"
}

Write-Host ""
Write-Host "Template installed safely. Existing files were not overwritten unless -Force was supplied."
Write-Host "Next: edit ai-dev.config.json, project-memory/SPEC.md, and project-memory/TASKS.md."
