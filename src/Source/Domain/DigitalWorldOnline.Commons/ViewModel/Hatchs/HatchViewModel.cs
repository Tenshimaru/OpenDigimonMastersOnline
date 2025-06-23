using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.ViewModel.Hatchs
{
    public class HatchViewModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Hatch grade type.
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

        /// <summary>
        /// Flag for invalid create configuration.
        /// </summary>
        public bool Invalid => SuccessChance == 0;

        public HatchViewModel()
        {
            SuccessChance = 1;
        }
    }
}
