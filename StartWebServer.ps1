#!/usr/bin/env pwsh
# OpenDigimonMastersServer - WebServer/Admin Launcher
# Standardized launcher script for WebServer/Admin with .env support

param(
    [switch]$NoWait,
    [switch]$Help
)

if ($Help) {
    Write-Host "OpenDigimonMastersServer - WebServer/Admin Launcher" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\StartWebServer.ps1 [options]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -NoWait    Start server without waiting for input"
    Write-Host "  -Help      Show this help message"
    Write-Host ""
    Write-Host "This script loads environment variables from .env file and starts the WebServer/Admin."
    exit 0
}

# Set console title and colors
$Host.UI.RawUI.WindowTitle = "OpenDigimonMastersServer - WebServer/Admin"
Write-Host "🌐 OpenDigimonMastersServer - WebServer/Admin Launcher" -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Gray

# Check if .env file exists
if (-not (Test-Path ".env")) {
    Write-Host "❌ Error: .env file not found!" -ForegroundColor Red
    Write-Host "💡 Please copy .env.example to .env and configure your settings." -ForegroundColor Yellow
    Write-Host ""
    if (-not $NoWait) {
        Write-Host "Press any key to exit..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
    exit 1
}

# Load environment variables from .env file
Write-Host "📁 Loading environment variables from .env..." -ForegroundColor Green

try {
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
    
    Write-Host "✅ Successfully loaded $variablesLoaded environment variables" -ForegroundColor Green
} catch {
    Write-Host "❌ Error loading .env file: $($_.Exception.Message)" -ForegroundColor Red
    if (-not $NoWait) {
        Write-Host "Press any key to exit..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
    exit 1
}

# Validate required environment variables
Write-Host ""
Write-Host "🔍 Validating configuration..." -ForegroundColor Yellow

$requiredVars = @("DMOX_CONNECTION_STRING")
$missingVars = @()

foreach ($var in $requiredVars) {
    $value = [Environment]::GetEnvironmentVariable($var)
    if ([string]::IsNullOrEmpty($value)) {
        $missingVars += $var
        Write-Host "  ❌ Missing: $var" -ForegroundColor Red
    } else {
        Write-Host "  ✅ Found: $var" -ForegroundColor Green
    }
}

if ($missingVars.Count -gt 0) {
    Write-Host ""
    Write-Host "❌ Missing required environment variables!" -ForegroundColor Red
    Write-Host "💡 Please configure the following variables in your .env file:" -ForegroundColor Yellow
    foreach ($var in $missingVars) {
        Write-Host "   - $var" -ForegroundColor Yellow
    }
    Write-Host ""
    if (-not $NoWait) {
        Write-Host "Press any key to exit..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
    exit 1
}

# Define server paths
$serverPath = "src\Source\Distribution\DigitalWorldOnline.Admin\bin\Release\net8.0\DigitalWorldOnline.Admin.exe"
$fallbackPath = "src\Source\Distribution\DigitalWorldOnline.Admin\bin\Debug\net8.0\DigitalWorldOnline.Admin.exe"

# Check if server executable exists
if (Test-Path $serverPath) {
    $executablePath = $serverPath
    $buildConfig = "Release"
} elseif (Test-Path $fallbackPath) {
    $executablePath = $fallbackPath
    $buildConfig = "Debug"
    Write-Host "⚠️  Using Debug build (Release build not found)" -ForegroundColor Yellow
} else {
    Write-Host "❌ Error: WebServer/Admin executable not found!" -ForegroundColor Red
    Write-Host "💡 Please build the solution first:" -ForegroundColor Yellow
    Write-Host "   dotnet build DigitalWorldOnline.sln --configuration Release" -ForegroundColor Gray
    Write-Host ""
    if (-not $NoWait) {
        Write-Host "Press any key to exit..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
    exit 1
}

# Display server information
Write-Host ""
Write-Host "🚀 Starting WebServer/Admin..." -ForegroundColor Green
Write-Host "📍 Executable: $executablePath" -ForegroundColor Gray
Write-Host "🔧 Build: $buildConfig" -ForegroundColor Gray
Write-Host "🌐 URLs: http://localhost:41001, http://localhost:5002" -ForegroundColor Gray
Write-Host ""

# Start the server
try {
    Write-Host "▶️  Launching WebServer/Admin..." -ForegroundColor Cyan
    Start-Process -FilePath $executablePath -WorkingDirectory (Get-Location)
    
    Write-Host "✅ WebServer/Admin started successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🌐 Access the admin panel at:" -ForegroundColor Cyan
    Write-Host "   • http://localhost:41001" -ForegroundColor White
    Write-Host "   • http://localhost:5002" -ForegroundColor White
    Write-Host ""
    
    if (-not $NoWait) {
        Write-Host "Press any key to exit..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
} catch {
    Write-Host "❌ Error starting WebServer/Admin: $($_.Exception.Message)" -ForegroundColor Red
    if (-not $NoWait) {
        Write-Host "Press any key to exit..." -ForegroundColor Gray
        $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
    exit 1
}
