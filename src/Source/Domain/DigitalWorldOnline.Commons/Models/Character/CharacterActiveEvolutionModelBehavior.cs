namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterActiveEvolutionModel
    {
        /// <summary>
        /// Updates the current DS usage.
        /// </summary>
        /// <param name="value">New DS usage.</param>
        public void SetDs(int value) => DsPerSecond = value;

        /// <summary>
        /// Updates the current XGauge usage.
        /// </summary>
        /// <param name="value">New XGauge usage.</param>
        public void SetXg(int value) => XgPerSecond = value;
    }
}
