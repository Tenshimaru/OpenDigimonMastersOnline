namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class NpcMobInfoAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }
        public int Round { get; set; }
        public int SummonType { get; set; }
        public int WinPoints { get; set; }
        public int LosePoints { get; set; }

    
    }
}