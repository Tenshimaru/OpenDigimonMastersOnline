using Microsoft.EntityFrameworkCore;
using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Shared;

namespace DigitalWorldOnline.Infrastructure
{
    public partial class DatabaseContext
    {
        public DbSet<ItemListDTO> ItemLists { get; set; }
        public DbSet<ItemDTO> Items { get; set; }
        public DbSet<ItemAccessoryStatusDTO> ItemAccsStatus { get; set; }
        public DbSet<ItemSocketStatusDTO> ItemSocketStatus { get; set; }

        internal static void SharedEntityConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ItemAccessoryStatusConfiguration());
            builder.ApplyConfiguration(new ItemSocketStatusConfiguration());
            builder.ApplyConfiguration(new ItemListConfiguration());
            builder.ApplyConfiguration(new ItemConfiguration());
        }
    }
}