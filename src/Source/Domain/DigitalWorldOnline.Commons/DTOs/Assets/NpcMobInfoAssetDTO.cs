namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class NpcMobInfoAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }
        public int Round { get; set; }
        public int SummonType { get; set; }
        public int WinPoints { get; set; }
        public int LosePoints { get; set; }
       
        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long NpcAssetId { get; set; }
        public NpcColiseumAssetDTO NpcAsset { get; set; }
    }
}