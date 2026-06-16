@echo off
REM Debug Run Script - Shows detailed output

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║  SmartPOS DEBUG RUN - Detailed Output                      ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

echo 📂 Current Directory:
cd
echo.

echo 🔍 Checking Files:
if exist "SmartPOS.UI\SmartPOS.UI.csproj" (
    echo ✅ SmartPOS.UI project found
) else (
    echo ❌ SmartPOS.UI project NOT found
    pause
    exit /b 1
)

if exist "SmartPOS.UI\appsettings.json" (
    echo ✅ appsettings.json exists
) else (
    echo ⚠️  appsettings.json NOT found (will be created on first run)
)

echo.
echo 🔨 Building...
dotnet build SmartPOS.UI\SmartPOS.UI.csproj

if %errorlevel% neq 0 (
    echo.
    echo ❌ Build failed!
    pause
    exit /b %errorlevel%
)

echo.
echo ✅ Build successful!
echo.
echo 🚀 LAUNCHING APPLICATION...
echo    (If window doesn't appear, check if it opened behind other windows)
echo.

REM Run the application with output capture
dotnet run --project SmartPOS.UI\SmartPOS.UI.csproj

echo.
echo ✅ Application closed
echo.
pause
