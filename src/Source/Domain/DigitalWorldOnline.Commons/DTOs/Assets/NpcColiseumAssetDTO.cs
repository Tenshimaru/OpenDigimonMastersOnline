namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class NpcColiseumAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the target NPC.
        /// </summary>
        public int NpcId { get; set; }

        /// <summary>
        /// Available item list.
        /// </summary>
        public List<NpcMobInfoAssetDTO> MobInfo { get; set; }

      
    }
}