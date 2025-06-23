using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public class CloneConfigDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Clone type.
        /// </summary>
        public DigicloneTypeEnum Type { get; set; }
        
        /// <summary>
        /// Clone target level.
        /// </summary>
        public byte Level { get; set; }
        
        /// <summary>
        /// Chance of success.
        /// </summary>
        public double SuccessChance { get; set; }
        
        /// <summary>
        /// Chance of break (on fail).
        /// </summary>
        public double BreakChance { get; set; }

        /// <summary>
        /// Min amount.
        /// </summary>
        public int MinAmount { get; set; }

        /// <summary>
        /// Max amount.
        /// </summary>
        public int MaxAmount { get; set; }
    }
}