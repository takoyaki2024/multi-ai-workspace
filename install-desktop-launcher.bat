@echo off
setlocal
cd /d "%~dp0"
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0scripts\install-desktop-launcher.ps1"
if errorlevel 1 (
  echo.
  echo Setup failed. Please send the error above to ChatGPT.
  pause
  exit /b 1
)
echo.
echo Setup complete.
pause
