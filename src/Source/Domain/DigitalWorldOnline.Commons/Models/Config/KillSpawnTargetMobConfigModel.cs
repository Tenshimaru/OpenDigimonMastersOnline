

using DigitalWorldOnline.Commons.Models.Base;

namespace DigitalWorldOnline.Commons.Models.Config
{
    public sealed class KillSpawnTargetMobConfigModel :ICloneable
    {
        public long Id { get; set; }

        public int TargetMobType { get; set; }

        public byte TargetMobAmount { get; set; }

        public object Clone()
        {
            
                return (KillSpawnTargetMobConfigModel)MemberwiseClone();        
        }
    }
}