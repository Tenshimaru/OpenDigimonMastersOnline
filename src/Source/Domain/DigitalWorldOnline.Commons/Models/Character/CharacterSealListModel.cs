namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterSealListModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Current seal leader id.
        /// </summary>
        public short SealLeaderId { get; private set; }

        /// <summary>
        /// Seals list.
        /// </summary>
        public IList<CharacterSealModel> Seals { get; private set; }

        public CharacterSealListModel()
        {
            Seals = new List<CharacterSealModel>();
        }
    }
}