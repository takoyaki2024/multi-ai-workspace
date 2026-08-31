[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$projectFile = Join-Path $projectRoot 'src\MultiAiWorkspace\MultiAiWorkspace.csproj'
$publishDirectory = Join-Path $env:LOCALAPPDATA 'MultiAiWorkspace\App'
$desktopDirectory = [Environment]::GetFolderPath('DesktopDirectory')
$shortcutPath = Join-Path $desktopDirectory 'Multi AI Workspace.lnk'
$executablePath = Join-Path $publishDirectory 'MultiAiWorkspace.exe'

Write-Host 'Publishing Multi AI Workspace...'
dotnet publish $projectFile --configuration Release --runtime win-x64 --self-contained false --output $publishDirectory
if ($LASTEXITCODE -ne 0) { throw 'Release publish failed.' }

if (-not (Test-Path -LiteralPath $executablePath)) { throw "Executable not found: $executablePath" }

$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $executablePath
$shortcut.WorkingDirectory = $publishDirectory
$shortcut.Description = 'Multi AI Workspace - ChatGPT, Gemini and Claude'
$shortcut.Save()

Write-Host ''
Write-Host 'Setup completed successfully.' -ForegroundColor Green
Write-Host "Desktop shortcut created: $shortcutPath"
