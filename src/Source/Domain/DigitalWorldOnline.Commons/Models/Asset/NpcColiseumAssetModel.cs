using DigitalWorldOnline.Commons.DTOs.Assets;

namespace DigitalWorldOnline.Commons.Models.Assets
{
    public sealed class NpcColiseumAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to the target NPC.
        /// </summary>
        public int NpcId { get; set; }

        public List<NpcMobInfoAssetModel> MobInfo { get; set; }
    }
}