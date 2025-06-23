namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterMapRegionModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Unlocked region.
        /// </summary>
        public byte Unlocked { get; private set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long CharacterId { get; set; }
    }
}