namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterIncubatorModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Reference ID to the hatching egg.
        /// </summary>
        public int EggId { get; private set; }

        /// <summary>
        /// Current hatch level for the egg.
        /// </summary>
        public int HatchLevel { get; private set; }

        /// <summary>
        /// Backup disk item id.
        /// </summary>
        public int BackupDiskId { get; private set; }

        /// <summary>
        /// Reference ID to character.
        /// </summary>
        public long CharacterId { get; private set; }
    }
}
