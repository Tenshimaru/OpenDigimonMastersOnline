using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class QuestRewardAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Quest reward. (?)
        /// </summary>
        public int Reward { get; set; }

        /// <summary>
        /// Quest reward type enumeration.
        /// </summary>
        public QuestRewardTypeEnum RewardType { get; set; }

        /// <summary>
        /// Quest reward list.
        /// </summary>
        public List<QuestRewardObjectAssetDTO> RewardObjectList { get; set; }

        /// <summary>
        /// Parent object.
        /// </summary>
        public long QuestId { get; set; }
        public QuestAssetDTO Quest { get; set; }
    }
}