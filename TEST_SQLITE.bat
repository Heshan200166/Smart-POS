@echo off
echo ========================================
echo Testing Smart POS with SQLite
echo ========================================
echo.
echo Building application...
dotnet build
echo.
echo ========================================
echo Starting application...
echo Watch for database initialization messages
echo ========================================
echo.
dotnet run --project SmartPOS.UI
pause