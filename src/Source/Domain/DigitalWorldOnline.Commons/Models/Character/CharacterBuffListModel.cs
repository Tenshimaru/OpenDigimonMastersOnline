namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterBuffListModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }
        
        /// <summary>
        /// Buffs inside the list.
        /// </summary>
        public List<CharacterBuffModel> Buffs { get; set; }

        public CharacterBuffListModel()
        {
            Buffs = new List<CharacterBuffModel>();
        }
    }
}