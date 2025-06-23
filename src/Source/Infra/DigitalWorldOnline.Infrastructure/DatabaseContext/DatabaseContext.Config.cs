using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.DTOs.Config.Events;
using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Config;
using DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.Events;
using Microsoft.EntityFrameworkCore;
using BitsDropRewardConfigConfiguration =
    DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.BitsDropRewardConfigConfiguration;
using ItemDropRewardConfigConfiguration =
    DigitalWorldOnline.Infrastructure.ContextConfiguration.Config.ItemDropRewardConfigConfiguration;

namespace DigitalWorldOnline.Infrastructure
{
    public partial class DatabaseContext : DbContext
    {
        public DbSet<ServerDTO> ServerConfig { get; set; }
        public DbSet<MapConfigDTO> MapConfig { get; set; }
        public DbSet<MobConfigDTO> MobConfig { get; set; }
        public DbSet<WelcomeMessageConfigDTO> WelcomeMessagesConfig { get; set; }
        public DbSet<UserDTO> UserConfig { get; set; }
        public DbSet<HashDTO> HashConfig { get; set; }
        public DbSet<CloneConfigDTO> CloneConfig { get; set; }
        public DbSet<HatchConfigDTO> HatchConfig { get; set; }
        public DbSet<FruitConfigDTO> FruitConfig { get; set; }
        public DbSet<SummonDTO> SummonsConfig { get; set; }
        public DbSet<SummonMobDTO> SummonsMobConfig { get; set; }

        public DbSet<EventConfigDTO> EventConfig { get; set; }
        
        public DbSet<EventMapsConfigDTO> EventMapsConfig { get; set; }
        
        public DbSet<EventMobConfigDTO> EventMobConfig { get; set; }

        internal static void ConfigEntityConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ServerConfigConfiguration());
            builder.ApplyConfiguration(new MapConfigConfiguration());
            builder.ApplyConfiguration(new MobConfigConfiguration());
            builder.ApplyConfiguration(new KillSpawnConfigConfiguration());
            builder.ApplyConfiguration(new MobExpRewardConfigConfiguration());
            builder.ApplyConfiguration(new MobLocationConfigConfiguration());
            builder.ApplyConfiguration(new MobDropRewardConfigConfiguration());
            builder.ApplyConfiguration(new ItemDropRewardConfigConfiguration());
            builder.ApplyConfiguration(new BitsDropRewardConfigConfiguration());
            builder.ApplyConfiguration(new SummonConfiguration());
            builder.ApplyConfiguration(new SummonMobConfiguration());
            builder.ApplyConfiguration(new SummonMobExpRewardConfiguration());
            builder.ApplyConfiguration(new SummonMobLocationConfiguration());
            builder.ApplyConfiguration(new SummonMobDropRewardConfiguration());
            builder.ApplyConfiguration(new SummonMobItemDropRewardConfiguration());
            builder.ApplyConfiguration(new SummonMobBitDropRewardConfiguration());
            builder.ApplyConfiguration(new WelcomeMessageConfigConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new HashConfiguration());
            builder.ApplyConfiguration(new CloneConfiguration());
            builder.ApplyConfiguration(new HatchConfiguration());
            builder.ApplyConfiguration(new FruitConfiguration());
            builder.ApplyConfiguration(new FruitSizeConfiguration());
            builder.ApplyConfiguration(new KillSpawnTargetMobConfigConfiguration());
            builder.ApplyConfiguration(new KillSpawnSourceMobConfigConfiguration());

            builder.ApplyConfiguration(new EventConfigConfiguration());
            builder.ApplyConfiguration(new EventMapsConfigConfiguration());
            builder.ApplyConfiguration(new EventMobConfigConfiguration());
            builder.ApplyConfiguration(new EventMobExpRewardConfigConfiguration());
            builder.ApplyConfiguration(new EventMobLocationConfigConfiguration());
            builder.ApplyConfiguration(new EventMobDropRewardConfigConfiguration());
            builder.ApplyConfiguration(new EventItemDropRewardConfigConfiguration());
            builder.ApplyConfiguration(new EventBitsDropRewardConfigConfiguration());
        }
    }
}