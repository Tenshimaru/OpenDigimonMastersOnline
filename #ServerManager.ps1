# OpenDigimonMastersServer Manager
# PowerShell script with clean interface and proper environment variable handling

param(
    [string]$Action = "menu"
)

# Set console encoding to UTF-8 for proper emoji display
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$Host.UI.RawUI.WindowTitle = "OpenDigimonMastersServer Manager"

# Global variables for server processes
$global:ServerProcesses = @{}

function Write-Header {
    Clear-Host
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "   OpenDigimonMastersServer Manager" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
}

function Load-EnvironmentVariables {
    Write-Host "Loading environment variables from .env file..." -ForegroundColor Yellow
    
    if (-not (Test-Path ".env")) {
        Write-Host "ERROR: .env file not found!" -ForegroundColor Red
        Write-Host "Please copy .env.example to .env and configure your settings" -ForegroundColor Yellow
        return $false
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
                Write-Host "  Loaded: $name" -ForegroundColor Gray
                $variablesLoaded++
            }
        }
    }
    
    Write-Host "Environment variables loaded successfully! ($variablesLoaded variables)" -ForegroundColor Green
    
    # Verify critical variables
    $dbConnection = [Environment]::GetEnvironmentVariable("DMOX_CONNECTION_STRING")
    if ($dbConnection) {
        $maskedConnection = $dbConnection -replace "Password=[^;]*", "Password=***"
        Write-Host "Database: $($maskedConnection.Substring(0, [Math]::Min(50, $maskedConnection.Length)))..." -ForegroundColor Cyan
    } else {
        Write-Host "WARNING: DMOX_CONNECTION_STRING not found in .env file!" -ForegroundColor Yellow
        return $false
    }
    
    Write-Host ""
    return $true
}

function Get-ServerConfig {
    return @{
        "Authentication" = @{
            Path = "src\Source\Distribution\DigitalWorldOnline.Account.Host\bin\Release\net8.0\DigitalWorldOnline.Account.exe"
            Name = "Authentication Server"
            Icon = "🔐"
            Port = "7029"
        }
        "Character" = @{
            Path = "src\Source\Distribution\DigitalWorldOnline.Character.Host\bin\Release\net8.0\DigitalWorldOnline.Character.exe"
            Name = "Character Server"
            Icon = "👤"
            Port = "7050"
        }
        "Game" = @{
            Path = "src\Source\Distribution\DigitalWorldOnline.Game.Host\bin\Release\net8.0\DigitalWorldOnline.Game.exe"
            Name = "Game Server"
            Icon = "🎮"
            Port = "7607"
        }
        "Routine" = @{
            Path = "src\Source\Distribution\DigitalWorldOnline.Routine.Host\DigitalWorldOnline.Routine\bin\Release\net8.0\DigitalWorldOnline.Routine.exe"
            Name = "Routine Server"
            Icon = "⚙️"
            Port = "Background"
        }
        "WebServer" = @{
            Path = "src\Source\Distribution\DigitalWorldOnline.Admin\bin\Release\net8.0\DigitalWorldOnline.Admin.exe"
            Name = "WebServer/Admin"
            Icon = "🌐"
            Port = "41001/5002"
        }
    }
}

function Start-Server {
    param(
        [string]$ServerType,
        [hashtable]$Config
    )
    
    Write-Host "$($Config.Icon) Starting $($Config.Name)..." -ForegroundColor Green
    
    if (-not (Test-Path $Config.Path)) {
        Write-Host "ERROR: Server executable not found at $($Config.Path)" -ForegroundColor Red
        return $false
    }
    
    try {
        # Get the directory of the executable
        $serverDir = Split-Path $Config.Path -Parent
        
        # Start the server process
        $processInfo = New-Object System.Diagnostics.ProcessStartInfo
        $processInfo.FileName = $Config.Path
        $processInfo.WorkingDirectory = $serverDir
        $processInfo.UseShellExecute = $false
        $processInfo.CreateNoWindow = $false
        $processInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Normal

        # Copy environment variables to the process
        foreach ($envVar in [Environment]::GetEnvironmentVariables("Process").GetEnumerator()) {
            $processInfo.EnvironmentVariables[$envVar.Key] = $envVar.Value
        }
        
        $process = [System.Diagnostics.Process]::Start($processInfo)
        $global:ServerProcesses[$ServerType] = $process
        
        Write-Host "  Started successfully (PID: $($process.Id))" -ForegroundColor Green
        Start-Sleep -Seconds 2
        return $true
    }
    catch {
        Write-Host "ERROR: Failed to start $($Config.Name): $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Stop-Server {
    param([string]$ServerType)
    
    if ($global:ServerProcesses.ContainsKey($ServerType)) {
        $process = $global:ServerProcesses[$ServerType]
        if (-not $process.HasExited) {
            Write-Host "Stopping $ServerType server..." -ForegroundColor Yellow
            $process.Kill()
            $process.WaitForExit(5000)
        }
        $global:ServerProcesses.Remove($ServerType)
        Write-Host "$ServerType server stopped." -ForegroundColor Green
    }
}

function Get-ServerStatus {
    $servers = Get-ServerConfig
    
    Write-Host "Server Status:" -ForegroundColor Cyan
    Write-Host "----------------------------------------" -ForegroundColor Gray
    
    foreach ($serverType in $servers.Keys) {
        $config = $servers[$serverType]
        $status = "Stopped"
        $color = "Red"
        
        if ($global:ServerProcesses.ContainsKey($serverType)) {
            $process = $global:ServerProcesses[$serverType]
            if (-not $process.HasExited) {
                $status = "Running (PID: $($process.Id))"
                $color = "Green"
            } else {
                $global:ServerProcesses.Remove($serverType)
            }
        }
        
        Write-Host "$($config.Icon) $($config.Name): " -NoNewline -ForegroundColor White
        Write-Host $status -ForegroundColor $color
    }
    
    Write-Host ""
}

function Show-Menu {
    Write-Header
    
    if (-not (Load-EnvironmentVariables)) {
        Read-Host "Press Enter to exit"
        return
    }
    
    do {
        Write-Host "Choose an option:" -ForegroundColor Yellow
        Write-Host "1. Start All Servers" -ForegroundColor White
        Write-Host "2. Start Individual Server" -ForegroundColor White
        Write-Host "3. Stop All Servers" -ForegroundColor White
        Write-Host "4. Stop Individual Server" -ForegroundColor White
        Write-Host "5. Show Server Status" -ForegroundColor White
        Write-Host "6. Reload Environment Variables" -ForegroundColor White
        Write-Host "0. Exit" -ForegroundColor White
        Write-Host ""
        
        Get-ServerStatus
        
        $choice = Read-Host "Enter your choice (0-6)"
        
        switch ($choice) {
            "1" { Start-AllServers }
            "2" { Start-IndividualServer }
            "3" { Stop-AllServers }
            "4" { Stop-IndividualServer }
            "5" { Get-ServerStatus; Read-Host "Press Enter to continue" }
            "6" { Load-EnvironmentVariables; Read-Host "Press Enter to continue" }
            "0" { 
                Stop-AllServers
                Write-Host "Goodbye!" -ForegroundColor Green
                return 
            }
            default { 
                Write-Host "Invalid option!" -ForegroundColor Red
                Start-Sleep -Seconds 1
            }
        }
        
        Write-Host ""
    } while ($true)
}

function Start-AllServers {
    Write-Host "Starting all servers..." -ForegroundColor Cyan
    Write-Host ""
    
    $servers = Get-ServerConfig
    $startedCount = 0
    
    foreach ($serverType in $servers.Keys) {
        if (Start-Server -ServerType $serverType -Config $servers[$serverType]) {
            $startedCount++
        }
    }
    
    Write-Host ""
    Write-Host "Started $startedCount out of $($servers.Count) servers." -ForegroundColor Cyan
    Read-Host "Press Enter to continue"
}

function Start-IndividualServer {
    $servers = Get-ServerConfig
    
    Write-Host "Choose server to start:" -ForegroundColor Yellow
    $i = 1
    $serverList = @()
    
    foreach ($serverType in $servers.Keys) {
        $config = $servers[$serverType]
        Write-Host "$i. $($config.Icon) $($config.Name)" -ForegroundColor White
        $serverList += $serverType
        $i++
    }
    
    $choice = Read-Host "Enter server number (1-$($servers.Count))"
    
    try {
        $index = [int]$choice - 1
        if ($index -ge 0 -and $index -lt $serverList.Count) {
            $serverType = $serverList[$index]
            Start-Server -ServerType $serverType -Config $servers[$serverType]
        } else {
            Write-Host "Invalid choice!" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "Invalid input!" -ForegroundColor Red
    }
    
    Read-Host "Press Enter to continue"
}

function Stop-AllServers {
    Write-Host "Stopping all servers..." -ForegroundColor Yellow
    
    $serverTypes = @($global:ServerProcesses.Keys)
    foreach ($serverType in $serverTypes) {
        Stop-Server -ServerType $serverType
    }
    
    if ($serverTypes.Count -eq 0) {
        Write-Host "No servers are currently running." -ForegroundColor Gray
    }
}

function Stop-IndividualServer {
    if ($global:ServerProcesses.Count -eq 0) {
        Write-Host "No servers are currently running." -ForegroundColor Gray
        Read-Host "Press Enter to continue"
        return
    }
    
    Write-Host "Choose server to stop:" -ForegroundColor Yellow
    $i = 1
    $runningServers = @()
    
    foreach ($serverType in $global:ServerProcesses.Keys) {
        $servers = Get-ServerConfig
        $config = $servers[$serverType]
        Write-Host "$i. $($config.Icon) $($config.Name)" -ForegroundColor White
        $runningServers += $serverType
        $i++
    }
    
    $choice = Read-Host "Enter server number (1-$($runningServers.Count))"
    
    try {
        $index = [int]$choice - 1
        if ($index -ge 0 -and $index -lt $runningServers.Count) {
            $serverType = $runningServers[$index]
            Stop-Server -ServerType $serverType
        } else {
            Write-Host "Invalid choice!" -ForegroundColor Red
        }
    }
    catch {
        Write-Host "Invalid input!" -ForegroundColor Red
    }
    
    Read-Host "Press Enter to continue"
}

# Main execution
if ($Action -eq "menu") {
    Show-Menu
} else {
    Write-Host "OpenDigimonMastersServer Manager" -ForegroundColor Cyan
    Write-Host "Usage: .\ServerManager.ps1" -ForegroundColor White
}
