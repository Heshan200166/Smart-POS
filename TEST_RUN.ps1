# Test Run Script with Verbose Output
# Right-click and "Run with PowerShell"

Write-Host "`n╔════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Smart POS - Application Test Run             ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

# Check if we're in the right directory
$projectFile = "SmartPOS.UI\SmartPOS.UI.csproj"
if (-not (Test-Path $projectFile)) {
    Write-Host "❌ Error: Cannot find project!" -ForegroundColor Red
    Write-Host "Current location: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}

Write-Host "📂 Project location: $(Get-Location)" -ForegroundColor Green
Write-Host "✅ Project found`n" -ForegroundColor Green

# Check appsettings.json
Write-Host "🔍 Checking configuration..." -ForegroundColor Yellow
if (Test-Path "SmartPOS.UI\appsettings.json") {
    Write-Host "✅ appsettings.json found" -ForegroundColor Green
} else {
    Write-Host "⚠️  appsettings.json not found (will be created on first run)" -ForegroundColor Yellow
}

Write-Host "`n🔨 Building project..." -ForegroundColor Yellow
dotnet build SmartPOS.UI\SmartPOS.UI.csproj --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`n✅ Build successful!`n" -ForegroundColor Green

Write-Host "🚀 Starting application with output..." -ForegroundColor Yellow
Write-Host "────────────────────────────────────────────────" -ForegroundColor Gray
Write-Host ""

# Run with error handling
try {
    & dotnet run --project SmartPOS.UI\SmartPOS.UI.csproj --verbosity minimal
}
catch {
    Write-Host "`n❌ Error: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "────────────────────────────────────────────────" -ForegroundColor Gray
Write-Host "`n✅ Application run completed!" -ForegroundColor Green

Read-Host "Press Enter to close this window"
