# 🎮 OpenDigimonMastersOnline (ODMO)

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red.svg)](https://www.microsoft.com/en-us/sql-server)

OpenDigimonMastersOnline is an open-source server implementation for the popular MMORPG Digimon Masters Online. This project provides a complete server infrastructure including authentication, character management, game world, and administrative tools.

## 🌟 Features

- **Complete Server Infrastructure**: Authentication, Character, Game, and Admin servers
- **Web-based Administration Panel**: Modern Blazor-based admin interface
- **Database Management**: Entity Framework Core with SQL Server support
- **Real-time Game World**: Multi-threaded game server with event handling
- **Player Management**: Character creation, progression, and inventory systems
- **Digimon System**: Complete Digimon management, evolution, and battle mechanics
- **Event System**: Configurable in-game events and raids
- **Security**: Hash-based authentication and anti-cheat measures

## 🏗️ Architecture

The project follows a clean architecture pattern with multiple layers:

```
├── Distribution Layer (Servers)
│   ├── Authentication Server (Port 7606)
│   ├── Character Server (Port 7050)
│   ├── Game Server (Port 7607)
│   ├── Admin Web Server (Port 5001/5002)
│   └── Gateway Server (Port 8074)
├── Application Layer (Business Logic)
├── Domain Layer (Core Models)
└── Infrastructure Layer (Database & External Services)
```

## 📋 Prerequisites

Before setting up ODMO, ensure you have the following installed:

- **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** or later
- **[SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)** (Express, Developer, or Standard)
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** or **[Visual Studio Code](https://code.visualstudio.com/)** (recommended)
- **Windows 10/11** or **Linux** (with SQL Server support)

### Verify Prerequisites

```powershell
# Check .NET SDK version
dotnet --version
# Should show 8.x.x or later

# Check SQL Server connectivity
sqlcmd -S "localhost\SQLEXPRESS" -E -Q "SELECT @@VERSION"
```

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/your-repo/OpenDigimonMastersOnline.git
cd OpenDigimonMastersOnline
```

### 2. Database Setup

#### Option A: Environment Configuration (Recommended)

1. Copy the environment template:
```powershell
copy .env.example .env
```

2. Edit `.env` with your database settings:
```bash
# Windows Authentication (Recommended for development)
DMOX_CONNECTION_STRING=Server=localhost\SQLEXPRESS;Database=DMOX;Integrated Security=true;TrustServerCertificate=True

# Or SQL Server Authentication
# DMOX_CONNECTION_STRING=Server=localhost\SQLEXPRESS;Database=DMOX;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True
```

#### Option B: User Secrets (Alternative)

```powershell
dotnet user-secrets set "ConnectionStrings:Digimon" "Server=localhost\SQLEXPRESS;Database=DMOX;Integrated Security=true;TrustServerCertificate=True"
```

### 3. Build the Solution

```powershell
# Clean and restore packages
dotnet clean
dotnet restore

# Build the entire solution
dotnet build --configuration Release
```

### 4. Database Migration

The database will be automatically created and migrated when you start the Authentication Server for the first time. Alternatively, you can run migrations manually:

```powershell
# Navigate to any project with DatabaseContext
cd "src\Source\Distribution\DigitalWorldOnline.Account.Host"

# Run migrations
dotnet ef database update
```

### 5. Start the Servers

#### Option A: All Servers in One Window
```powershell
.\StartAllServers.ps1
```

#### Option B: Separate Windows for Each Server
```powershell
.\StartServers-Separate Windows.ps1
```

#### Option C: Individual Server Management
```powershell
.\ServerManager.ps1
```

### 6. Start the Web Admin Panel (Optional)

```powershell
.\StartWebServer.ps1
```

## 🎯 Default Access Information

### Game Client Connection
- **Server Address**: `localhost` or `127.0.0.1`
- **Port**: `7607`

### Default Login Credentials
- **Username**: `Tenshimaru`
- **Password**: `123456`

### Admin Panel Access
- **URL**: `https://localhost:5001` or `https://localhost:5002`
- **Swagger API**: `https://localhost:5001/swagger`

## 🗄️ Database Schema

The project uses Entity Framework Core with SQL Server and includes:

- **Account Management**: User accounts, authentication, and security
- **Character System**: Player characters, stats, and progression
- **Digimon System**: Digimon data, evolution trees, and abilities
- **Game World**: Maps, NPCs, items, and quests
- **Configuration**: Server settings, events, and game parameters

## 🔧 Configuration

### Server Ports

| Server | Default Port | Purpose |
|--------|-------------|---------|
| Authentication | 7606 | Player login and account management |
| Character | 7050 | Character selection and creation |
| Game | 7607 | Main game world and gameplay |
| Admin Web | 5001/5002 | Web-based administration |
| Gateway | 8074 | Server communication gateway |

### Environment Variables

Key environment variables for configuration:

```bash
# Database
DMOX_CONNECTION_STRING=<your_connection_string>

# Game Server
GAME_SERVER_ADDRESS=0.0.0.0          # Server binding address
GAME_SERVER_PORT=7607
GAME_SERVER_PUBLIC_ADDRESS=127.0.0.1  # Client connection address

# Authentication Server
AUTH_SERVER_ADDRESS=0.0.0.0          # Server binding address
AUTH_SERVER_PORT=7606

# Character Server
CHARACTER_SERVER_ADDRESS=0.0.0.0     # Server binding address
CHARACTER_SERVER_PORT=7608
```

## 🛠️ Development

### Project Structure

```
OpenDigimonMastersOnline/
├── src/Source/
│   ├── Distribution/          # Server applications
│   │   ├── DigitalWorldOnline.Account.Host/
│   │   ├── DigitalWorldOnline.Character.Host/
│   │   ├── DigitalWorldOnline.Game.Host/
│   │   ├── DigitalWorldOnline.Admin/
│   │   └── DigitalWorldOnline.Gateway/
│   ├── Application/           # Business logic layer
│   ├── Domain/               # Core domain models
│   └── Infra/               # Infrastructure layer
├── docs/                    # Documentation
├── logs/                    # Server logs
└── scripts/                 # Utility scripts
```

### Building from Source

```powershell
# Debug build
dotnet build --configuration Debug

# Release build
dotnet build --configuration Release

# Clean build
dotnet clean && dotnet restore && dotnet build
```

### Running Tests

```powershell
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🐛 Troubleshooting

### Common Issues

#### Build Errors
```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

#### Database Connection Issues
1. Verify SQL Server is running
2. Check connection string in `.env` file
3. Ensure database permissions are correct
4. Test connection: `sqlcmd -S "localhost\SQLEXPRESS" -E -Q "SELECT 1"`

#### Server Won't Start
1. Check if ports are available (7606-7608, 5001-5002)
2. Verify all executables exist in `bin/Release/net8.0/`
3. Check logs in `logs/` directory
4. Ensure environment variables are loaded

#### Client Connection Issues
1. Verify game server is running on port 7607
2. Check firewall settings
3. Ensure client is configured to connect to correct IP/port

### Log Files

Server logs are stored in the `logs/` directory:
- `logs/Account/` - Authentication server logs
- `logs/Character/` - Character server logs  
- `logs/Game/` - Game server logs
- `logs/Debug/` - Debug information
- `logs/Error/` - Error logs

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE.txt](LICENSE.txt) file for details.

## 🙏 Acknowledgments

- Original Digimon Masters Online developers
- The open-source gaming community
- Contributors and testers

## 📞 Support

- **Discord**: [Join our Discord](https://discord.gg/VcNuqrW3WH)
- **Issues**: [GitHub Issues](https://github.com/your-repo/OpenDigimonMastersOnline/issues)
- **Documentation**: [Wiki](https://github.com/your-repo/OpenDigimonMastersOnline/wiki)

---

**Note**: This is an unofficial open-source implementation. All Digimon-related trademarks and copyrights belong to their respective owners.
