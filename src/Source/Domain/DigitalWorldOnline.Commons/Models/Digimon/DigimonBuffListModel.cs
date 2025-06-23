namespace DigitalWorldOnline.Commons.Models.Digimon
{
    public sealed partial class DigimonBuffListModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }
        
        /// <summary>
        /// Buffs inside the list.
        /// </summary>
        public List<DigimonBuffModel> Buffs { get; set; }

        public DigimonBuffListModel()
        {
            Buffs = new List<DigimonBuffModel>();
        }
    }
}