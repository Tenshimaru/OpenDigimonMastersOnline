using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.DTOs.Config
{
    public class HatchConfigDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Hatch type.
        /// </summary>
        public HatchTypeEnum Type { get; set; }
        
        /// <summary>
        /// Chance of success.
        /// </summary>
        public double SuccessChance { get; set; }
        
        /// <summary>
        /// Chance of break (on fail).
        /// </summary>
        public double BreakChance { get; set; }
    }
}