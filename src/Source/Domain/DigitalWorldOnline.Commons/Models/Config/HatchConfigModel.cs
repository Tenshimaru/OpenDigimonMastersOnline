using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Config
{
    public class HatchConfigModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Hatch type.
        /// </summary>
        public HatchTypeEnum Type { get; private set; }

        /// <summary>
        /// Chance of success.
        /// </summary>
        public double SuccessChance { get; private set; }

        /// <summary>
        /// Chance of break (on fail).
        /// </summary>
        public double BreakChance { get; private set; }
    }
}