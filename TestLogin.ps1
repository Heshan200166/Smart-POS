# Quick test to check database users
Write-Host "Testing database and users..." -ForegroundColor Cyan

$dbPath = "SmartPOS.db"

if (Test-Path $dbPath) {
    Write-Host "✅ Database file exists: $dbPath" -ForegroundColor Green
    
    # Load SQLite assembly
    Add-Type -Path "$env:USERPROFILE\.nuget\packages\microsoft.data.sqlite\10.0.9\lib\net8.0\Microsoft.Data.Sqlite.dll" -ErrorAction SilentlyContinue
    
    try {
        $connectionString = "Data Source=$dbPath"
        $connection = New-Object Microsoft.Data.Sqlite.SqliteConnection($connectionString)
        $connection.Open()
        
        # Check users
        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT COUNT(*) FROM Users"
        $userCount = $command.ExecuteScalar()
        
        Write-Host "📊 Total users: $userCount" -ForegroundColor Yellow
        
        # List users
        $command.CommandText = "SELECT Username, IsActive FROM Users"
        $reader = $command.ExecuteReader()
        
        Write-Host "`n📋 Users in database:" -ForegroundColor Yellow
        while ($reader.Read()) {
            $username = $reader.GetString(0)
            $isActive = $reader.GetInt32(1)
            $activeText = if ($isActive -eq 1) { "Active" } else { "Inactive" }
            Write-Host "   - $username ($activeText)"
        }
        $reader.Close()
        
        $connection.Close()
    }
    catch {
        Write-Host "❌ Error reading database: $_" -ForegroundColor Red
    }
} else {
    Write-Host "❌ Database file not found: $dbPath" -ForegroundColor Red
}

Write-Host "`n📝 Instructions:" -ForegroundColor Cyan
Write-Host "1. If users exist, try login with: admin / admin123"
Write-Host "2. If no users or 0 count, run the app and click 'Initialize Database'"
Write-Host "3. Check console output when you try to login"
