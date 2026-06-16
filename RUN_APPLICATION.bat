@echo off
echo.
echo ╔══════════════════════════════════════════════════════════╗
echo ║                                                          ║
echo ║     🚀 Smart Retail POS Management System 🚀             ║
echo ║                                                          ║
echo ╚══════════════════════════════════════════════════════════╝
echo.
echo 📱 Starting application...
echo.

REM Build the solution
echo 🔨 Building solution...
dotnet build SmartPOS.UI\SmartPOS.UI.csproj

if %errorlevel% neq 0 (
    echo.
    echo ❌ Build failed! Please check the errors above.
    echo.
    pause
    exit /b %errorlevel%
)

echo.
echo ✅ Build successful!
echo.
echo 🚀 Launching Smart POS Application...
echo.
echo    The application window will open shortly...
echo    Close this window after the application starts.
echo.

REM Run the application
dotnet run --project SmartPOS.UI\SmartPOS.UI.csproj

if %errorlevel% neq 0 (
    echo.
    echo ❌ Application failed to start!
    echo.
    pause
    exit /b %errorlevel%
)

exit /b 0
