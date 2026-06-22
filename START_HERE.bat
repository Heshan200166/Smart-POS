@echo off
cls
echo.
echo ========================================================================
echo                    SMART POS - STARTING APPLICATION
echo ========================================================================
echo.
echo Please wait while the application starts...
echo.
echo What will happen:
echo   1. Database will be initialized (SmartPOS.db)
echo   2. Default users will be created
echo   3. Login window will appear
echo.
echo Login with:
echo   Username: admin
echo   Password: admin123
echo.
echo Or just click the "Admin" button!
echo.
echo ========================================================================
echo.

cd /d "%~dp0"
dotnet run --project SmartPOS.UI

echo.
echo ========================================================================
echo Application closed.
echo ========================================================================
pause