# Smart POS Application Launcher
# PowerShell Script - Right-click and "Run with PowerShell"

Write-Host "`n" -ForegroundColor Green
Write-Host "╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║                                                                ║" -ForegroundColor Cyan
Write-Host "║        🚀 Smart Retail POS Management System 🚀               ║" -ForegroundColor Green
Write-Host "║                                                                ║" -ForegroundColor Cyan
Write-Host "║              Starting Application...                          ║" -ForegroundColor Yellow
Write-Host "║                                                                ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host "`n"

# Check if we're in the right directory
$projectFile = "SmartPOS.UI\SmartPOS.UI.csproj"
if (-not (Test-Path $projectFile)) {
    Write-Host "❌ Error: Cannot find SmartPOS.UI project!" -ForegroundColor Red
    Write-Host "`nMake sure you're in: D:\Smart-POS\Smart-POS\" -ForegroundColor Yellow
    Write-Host "Current location: $(Get-Location)" -ForegroundColor Yellow
    Write-Host "`nPress Enter to exit..."
    Read-Host
    exit 1
}

Write-Host "✅ Project found!" -ForegroundColor Green
Write-Host "`n🔨 Building solution..." -ForegroundColor Yellow
Write-Host ""

# Build the solution
dotnet build SmartPOS.UI\SmartPOS.UI.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n❌ Build failed!" -ForegroundColor Red
    Write-Host "Press Enter to exit..."
    Read-Host
    exit 1
}

Write-Host "`n✅ Build successful!" -ForegroundColor Green
Write-Host "`n🚀 Launching application..." -ForegroundColor Green
Write-Host "`n   The application window will open in a moment...`n" -ForegroundColor Cyan

# Run the application
dotnet run --project SmartPOS.UI\SmartPOS.UI.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n❌ Application failed to start!" -ForegroundColor Red
    Write-Host "Press Enter to exit..."
    Read-Host
    exit 1
}

Write-Host "`n✅ Application closed successfully!" -ForegroundColor Green
Write-Host "`nPress Enter to exit..."
Read-Host
