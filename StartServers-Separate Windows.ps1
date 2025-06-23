# PowerShell script that creates separate windows for each server
# Uses cmd to launch servers in new windows with environment variables

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   OpenDigimonMastersServer Launcher" -ForegroundColor Cyan
Write-Host "   (Separate Windows Version)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Load environment variables from .env file
Write-Host "Loading environment variables from .env file..." -ForegroundColor Yellow

if (-not (Test-Path ".env")) {
    Write-Host "ERROR: .env file not found!" -ForegroundColor Red
    Write-Host "Please copy .env.example to .env and configure your settings" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

$envContent = Get-Content ".env" -Encoding UTF8
$envVars = @{}
$variablesLoaded = 0

foreach ($line in $envContent) {
    if ($line -and !$line.StartsWith("#") -and $line.Contains("=")) {
        $parts = $line.Split("=", 2)
        if ($parts.Length -eq 2) {
            $name = $parts[0].Trim()
            $value = $parts[1].Trim()
            
            # Remove quotes if present
            if ($value.StartsWith('"') -and $value.EndsWith('"')) {
                $value = $value.Substring(1, $value.Length - 2)
            }
            
            $envVars[$name] = $value
            [Environment]::SetEnvironmentVariable($name, $value, "Process")
            Write-Host "  ✓ Loaded: $name" -ForegroundColor Gray
            $variablesLoaded++
        }
    }
}

Write-Host "✅ Environment variables loaded successfully! ($variablesLoaded variables)" -ForegroundColor Green

# Verify database connection string
$dbConnection = $envVars["DMOX_CONNECTION_STRING"]
if ($dbConnection) {
    $maskedConnection = $dbConnection -replace "Password=[^;]*", "Password=***"
    Write-Host "🗄️ Database: $($maskedConnection.Substring(0, [Math]::Min(50, $maskedConnection.Length)))..." -ForegroundColor Cyan
} else {
    Write-Host "❌ WARNING: DMOX_CONNECTION_STRING not found in .env file!" -ForegroundColor Yellow
}

Write-Host ""

# Server configurations
$servers = @(
    @{
        Name = "Authentication Server"
        Path = "src\Source\Distribution\DigitalWorldOnline.Account.Host\bin\Release\net8.0\DigitalWorldOnline.Account.exe"
        Icon = "🔐"
        WindowTitle = "UDMO - Authentication Server"
    },
    @{
        Name = "Character Server"
        Path = "src\Source\Distribution\DigitalWorldOnline.Character.Host\bin\Release\net8.0\DigitalWorldOnline.Character.exe"
        Icon = "👤"
        WindowTitle = "UDMO - Character Server"
    },
    @{
        Name = "Game Server"
        Path = "src\Source\Distribution\DigitalWorldOnline.Game.Host\bin\Release\net8.0\DigitalWorldOnline.Game.exe"
        Icon = "🎮"
        WindowTitle = "UDMO - Game Server"
    },
    @{
        Name = "Routine Server"
        Path = "src\Source\Distribution\DigitalWorldOnline.Routine.Host\DigitalWorldOnline.Routine\bin\Release\net8.0\DigitalWorldOnline.Routine.exe"
        Icon = "⚙️"
        WindowTitle = "UDMO - Routine Server"
    }
)

Write-Host "🚀 Starting all servers with .env configuration..." -ForegroundColor Cyan
Write-Host ""

$startedServers = @()

# Create environment variable string for cmd
$envString = ""
foreach ($envVar in $envVars.GetEnumerator()) {
    $envString += "set `"$($envVar.Key)=$($envVar.Value)`" && "
}

foreach ($server in $servers) {
    Write-Host "$($server.Icon) Starting $($server.Name)..." -ForegroundColor Green
    
    if (-not (Test-Path $server.Path)) {
        Write-Host "   ❌ ERROR: Server executable not found at $($server.Path)" -ForegroundColor Red
        continue
    }
    
    try {
        # Get the directory of the executable
        $serverDir = Split-Path $server.Path -Parent
        $serverExe = Split-Path $server.Path -Leaf
        
        # Create command to run in new window
        $command = "cd /d `"$serverDir`" && $envString`"$serverExe`""
        
        # Start new cmd window with the server
        Start-Process -FilePath "cmd.exe" -ArgumentList "/c", "title $($server.WindowTitle) && $command && pause" -WindowStyle Normal
        
        $startedServers += @{
            Name = $server.Name
            Icon = $server.Icon
        }
        
        Write-Host "   ✅ Started successfully in new window" -ForegroundColor Green
        Start-Sleep -Seconds 2
    }
    catch {
        Write-Host "   ❌ ERROR: Failed to start $($server.Name): $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   🎉 Server Launch Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "📊 Started Servers:" -ForegroundColor Yellow
foreach ($server in $startedServers) {
    Write-Host "   $($server.Icon) $($server.Name): ✅ Running in separate window" -ForegroundColor White
}

Write-Host ""
Write-Host "📁 Configuration Source: .env file" -ForegroundColor Cyan
Write-Host "🗄️ Database: $($dbConnection.Split(';')[0])..." -ForegroundColor Cyan
Write-Host ""

Write-Host "💡 Tips:" -ForegroundColor Yellow
Write-Host "   • Each server runs in its own cmd window" -ForegroundColor White
Write-Host "   • Check individual server windows for logs" -ForegroundColor White
Write-Host "   • Look for 'Using connection string from environment variable'" -ForegroundColor White
Write-Host "   • Close server windows to stop servers" -ForegroundColor White
Write-Host "   • Edit .env file to change configuration" -ForegroundColor White
Write-Host ""

Write-Host "Press Enter to exit this launcher..." -ForegroundColor Gray
Read-Host
