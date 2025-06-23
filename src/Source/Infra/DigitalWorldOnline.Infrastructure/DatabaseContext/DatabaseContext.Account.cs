using DigitalWorldOnline.Commons.DTOs.Account;
using Microsoft.EntityFrameworkCore;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Config;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Account;

namespace DigitalWorldOnline.Infrastructure
{
    public partial class DatabaseContext
    {
        public DbSet<AccountDTO> Account { get; set; }
        public DbSet<AccountBlockDTO> AccountBlock { get; set; }
        public DbSet<SystemInformationDTO> SystemInformation { get; set; }

        internal static void AccountEntityConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AccountConfiguration());
            builder.ApplyConfiguration(new SystemInformationConfiguration());
            builder.ApplyConfiguration(new AccountBlockConfiguration());
        }
    }
}