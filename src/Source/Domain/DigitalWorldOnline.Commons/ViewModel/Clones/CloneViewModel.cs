using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.ViewModel.Clones
{
    public class CloneViewModel
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

        /// <summary>
        /// Flag for invalid create configuration.
        /// </summary>
        public bool Invalid => SuccessChance == 0 || MinAmount == 0 || MinAmount > MaxAmount;

        public CloneViewModel()
        {
            MinAmount = 1;
            MaxAmount = 1;
        }
    }
}
