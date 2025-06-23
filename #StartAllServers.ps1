# Simple PowerShell script to start all OpenDigimonMastersServer instances
# Reads configuration from .env file and manages server processes cleanly

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   OpenDigimonMastersServer Launcher" -ForegroundColor Cyan
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
            
            [Environment]::SetEnvironmentVariable($name, $value, "Process")
            Write-Host "  ✓ Loaded: $name" -ForegroundColor Gray
            $variablesLoaded++
        }
    }
}

Write-Host "✅ Environment variables loaded successfully! ($variablesLoaded variables)" -ForegroundColor Green

# Verify database connection string
$dbConnection = [Environment]::GetEnvironmentVariable("DMOX_CONNECTION_STRING")
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
    },
    @{
        Name = "WebServer/Admin"
        Path = "src\Source\Distribution\DigitalWorldOnline.Admin\bin\Release\net8.0\DigitalWorldOnline.Admin.exe"
        Icon = "🌐"
        WindowTitle = "UDMO - WebServer/Admin"
    }
)

Write-Host "🚀 Starting all servers with .env configuration..." -ForegroundColor Cyan
Write-Host ""

$startedServers = @()

foreach ($server in $servers) {
    Write-Host "$($server.Icon) Starting $($server.Name)..." -ForegroundColor Green
    
    if (-not (Test-Path $server.Path)) {
        Write-Host "   ❌ ERROR: Server executable not found at $($server.Path)" -ForegroundColor Red
        continue
    }
    
    try {
        # Get the directory of the executable
        $serverDir = Split-Path $server.Path -Parent
        
        # Start the server process with environment variables
        $processInfo = New-Object System.Diagnostics.ProcessStartInfo
        $processInfo.FileName = $server.Path
        $processInfo.WorkingDirectory = $serverDir
        $processInfo.UseShellExecute = $false
        $processInfo.CreateNoWindow = $false
        $processInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Normal

        # Copy environment variables to the process
        foreach ($envVar in [Environment]::GetEnvironmentVariables("Process").GetEnumerator()) {
            $processInfo.EnvironmentVariables[$envVar.Key] = $envVar.Value
        }

        $process = [System.Diagnostics.Process]::Start($processInfo)
        $startedServers += @{
            Name = $server.Name
            Process = $process
            Icon = $server.Icon
        }
        
        Write-Host "   ✅ Started successfully (PID: $($process.Id))" -ForegroundColor Green
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
    $status = if ($server.Process.HasExited) { "❌ Exited" } else { "✅ Running" }
    Write-Host "   $($server.Icon) $($server.Name): $status" -ForegroundColor White
}

Write-Host ""
Write-Host "📁 Configuration Source: .env file" -ForegroundColor Cyan
Write-Host "🗄️ Database: $($dbConnection.Split(';')[0])..." -ForegroundColor Cyan
Write-Host ""

Write-Host "💡 Tips:" -ForegroundColor Yellow
Write-Host "   • Each server runs in its own window" -ForegroundColor White
Write-Host "   • Check individual server windows for logs" -ForegroundColor White
Write-Host "   • Edit .env file to change configuration" -ForegroundColor White
Write-Host "   • Close server windows to stop servers" -ForegroundColor White
Write-Host "   • WebServer/Admin available at: http://localhost:41001" -ForegroundColor White
Write-Host ""

Write-Host "Press Enter to exit this launcher..." -ForegroundColor Gray
Read-Host
