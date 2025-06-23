namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterActiveEvolutionModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// DS usage per second.
        /// </summary>
        public int DsPerSecond { get; private set; }

        /// <summary>
        /// XGauge usage per second.
        /// </summary>
        public int XgPerSecond { get; private set; }
    }
}
