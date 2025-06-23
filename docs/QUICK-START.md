# 🚀 Quick Start Guide - OpenDigimonMastersServer

## ⚡ Fast Setup (5 minutes)

### 1. **Prerequisites Check**
```powershell
# Check .NET 8.0 SDK
dotnet --version
# Should show 8.x.x

# Check SQL Server
# Ensure SQL Server is running and accessible
```

### 2. **Environment Setup**
```powershell
# Copy environment template
copy .env.example .env

# Edit .env with your database settings
notepad .env

# Load environment variables
.\load-env.ps1
```

### 3. **Build & Run**
```powershell
# Automated migration to .NET 8.0 (if needed)
.\migrate-to-net8.ps1

# Start all servers
.\StartServer.bat
```

## 🔧 **Environment Configuration (.env)**

**Minimum required settings:**
```bash
# Database (Choose one option)
DMOX_CONNECTION_STRING=Server=localhost\SQLEXPRESS;Database=DMOX;Integrated Security=true;TrustServerCertificate=True

# For SQL Server Authentication
# DMOX_CONNECTION_STRING=Server=localhost\SQLEXPRESS;Database=DMOX;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True
```

## 🗄️ **Database Quick Setup**

### Option A: Import Backup
1. Open SQL Server Management Studio
2. Right-click Databases → Import Data-tier Application
3. Select `.bacpac` file from `Database/` folder

### Option B: Auto-Migration
1. Create empty database: `CREATE DATABASE DMOX;`
2. Start AccountServer first - it will create tables automatically

## 🎮 **Default Login**
- **Username**: `Tenshimaru`
- **Password**: `123456`

## 🌐 **Access Points**
- **Game Client**: Connect to `localhost:7607`
- **Admin Panel**: Run `StartWeb.bat` → `https://localhost:5001`
- **API**: `https://localhost:5001/swagger`

## 🛠️ **Troubleshooting**

### Build Errors
```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Database Connection Issues
```powershell
# Test connection string
sqlcmd -S "localhost\SQLEXPRESS" -E -Q "SELECT @@VERSION"
```

### Server Won't Start
1. Check logs in `logs/` directory
2. Verify all executables exist in `bin/Debug/net8.0/`
3. Ensure ports 7606-7608 are available

## 📁 **File Structure**
```
UDMO Server Source/
├── .env                    # Your environment config
├── .env.example           # Template
├── load-env.ps1          # Load environment variables
├── migrate-to-net8.ps1   # Migration script
├── StartServer.bat       # Start all servers (Windows)
├── start-server.sh       # Start all servers (Linux/macOS)
└── src/                  # Source code
```

## 🔄 **Common Commands**

```powershell
# Environment
.\load-env.ps1              # Load environment variables

# Servers
.\StartServer.bat           # Start all servers
.\StartWeb.bat             # Start web services

# Development
dotnet clean               # Clean builds
dotnet restore            # Restore packages
dotnet build              # Build solution

# Migration
.\migrate-to-net8.ps1     # Migrate to .NET 8.0
```

## 📞 **Need Help?**
- 📖 **Full Documentation**: `README.md`
- 🐛 **Issues**: Check `logs/` directory
- 💬 **Community**: [Project Repository]

---
**🎯 Goal**: Get your server running in under 5 minutes!
