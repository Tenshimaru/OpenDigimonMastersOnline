
using DigitalWorldOnline.Commons.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DigitalWorldOnline.Infrastructure
{
    public partial class DatabaseContext : DbContext
    {
        private const string DatabaseConnectionString = "Database:Connection";
        private readonly IConfiguration _configuration;
        private readonly bool _cliInitialization;
        private static bool _connectionStringLogged = false;
        private static readonly object _logLock = new object();

        public DatabaseContext()
        {
            _cliInitialization = true;
        }

        public DatabaseContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                string connectionString;

                // Priority order: Environment Variable > Configuration > Default
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DMOX_CONNECTION_STRING")))
                {
                    connectionString = Environment.GetEnvironmentVariable("DMOX_CONNECTION_STRING")!;
                    LogConnectionStringSource("✅ Using connection string from environment variable");
                }
                else if (_configuration != null && !string.IsNullOrEmpty(_configuration[DatabaseConnectionString]))
                {
                    connectionString = _configuration[DatabaseConnectionString]!;
                    LogConnectionStringSource("✅ Using connection string from configuration");
                }
                else if (_configuration != null && !string.IsNullOrEmpty(_configuration.GetConnectionString("Digimon")))
                {
                    connectionString = _configuration.GetConnectionString("Digimon")!;
                    LogConnectionStringSource("✅ Using connection string from ConnectionStrings:Digimon");
                }
                else
                {
                    // Default connection string for development
                    connectionString = "Server=localhost\\SQLEXPRESS;Database=DMOX;Integrated Security=true;TrustServerCertificate=True";
                    LogConnectionStringSource("⚠️  Using default connection string - consider setting DMOX_CONNECTION_STRING environment variable");
                }

                optionsBuilder.UseSqlServer(connectionString);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"❌ SQL Server connection error: {ex.Message}");
                Console.WriteLine("💡 Check your connection string and ensure SQL Server is running");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database configuration error: {ex.Message}");
                throw;
            }
        }

        private static void LogConnectionStringSource(string message)
        {
            lock (_logLock)
            {
                if (!_connectionStringLogged)
                {
                    Console.WriteLine(message);
                    _connectionStringLogged = true;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SharedEntityConfiguration(modelBuilder);
            AccountEntityConfiguration(modelBuilder);
            AssetsEntityConfiguration(modelBuilder);
            CharacterEntityConfiguration(modelBuilder);
            ConfigEntityConfiguration(modelBuilder);
            DigimonEntityConfiguration(modelBuilder);
            EventEntityConfiguration(modelBuilder);
            SecurityEntityConfiguration(modelBuilder);
            ShopEntityConfiguration(modelBuilder);
            MechanicsEntityConfiguration(modelBuilder);
            RoutineEntityConfiguration(modelBuilder);
            ArenaEntityConfiguration(modelBuilder);
        }
    }
}