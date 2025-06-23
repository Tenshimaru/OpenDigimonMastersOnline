using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Config
{
    public class CloneConfigModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Clone type.
        /// </summary>
        public DigicloneTypeEnum Type { get; private set; }

        /// <summary>
        /// Clone target level.
        /// </summary>
        public byte Level { get; private set; }

        /// <summary>
        /// Chance of success.
        /// </summary>
        public double SuccessChance { get; private set; }

        /// <summary>
        /// Chance of break (on fail).
        /// </summary>
        public double BreakChance { get; private set; }

        /// <summary>
        /// Flag for breakable clone.
        /// </summary>
        public bool CanBreak => BreakChance > 0;
    }
}