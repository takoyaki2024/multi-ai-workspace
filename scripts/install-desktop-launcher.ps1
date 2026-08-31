[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$projectFile = Join-Path $projectRoot 'src\MultiAiWorkspace\MultiAiWorkspace.csproj'
$publishDirectory = Join-Path $env:LOCALAPPDATA 'MultiAiWorkspace\App'
$desktopDirectory = [Environment]::GetFolderPath('DesktopDirectory')
$shortcutPath = Join-Path $desktopDirectory 'Multi AI Workspace.lnk'
$executablePath = Join-Path $publishDirectory 'MultiAiWorkspace.exe'

Write-Host 'Multi AI Workspace をReleaseビルドしています...'
dotnet publish $projectFile --configuration Release --runtime win-x64 --self-contained false --output $publishDirectory
if ($LASTEXITCODE -ne 0) { throw 'Release publish に失敗しました。' }

if (-not (Test-Path -LiteralPath $executablePath)) { throw "実行ファイルが見つかりません: $executablePath" }

$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $executablePath
$shortcut.WorkingDirectory = $publishDirectory
$shortcut.Description = 'ChatGPT・Gemini・Claudeを同時に使うデスクトップアプリ'
$shortcut.Save()

Write-Host ''
Write-Host 'セットアップが完了しました。' -ForegroundColor Green
Write-Host "デスクトップの「Multi AI Workspace」から起動できます。"
