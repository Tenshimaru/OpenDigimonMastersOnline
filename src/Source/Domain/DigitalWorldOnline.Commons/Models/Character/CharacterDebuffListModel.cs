namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterDebuffListModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Buffs inside the list.
        /// </summary>
        public List<CharacterDebuffModel> Buffs { get; set; }

        public CharacterDebuffListModel()
        {
            Buffs = new List<CharacterDebuffModel>();
        }
    }
}