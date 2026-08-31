@echo off
setlocal
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\install-desktop-launcher.ps1"
if errorlevel 1 (
  echo.
  echo セットアップに失敗しました。上のメッセージを確認してください。
  pause
  exit /b 1
)
echo.
pause
