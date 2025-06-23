namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterSealModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Client reference to the seal id.
        /// </summary>
        public int SealId { get; private set; }

        /// <summary>
        /// Current amount.
        /// </summary>
        public short Amount { get; private set; }

        /// <summary>
        /// Favorited seal.
        /// </summary>
        public bool Favorite { get; private set; }

        /// <summary>
        /// Client reference to the sequential seal id.
        /// </summary>
        public short SequentialId { get; private set; }

        public CharacterSealModel()
        {
            Id = Guid.NewGuid();
        }
    }
}