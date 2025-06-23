using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class QuestRewardAssetModel
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
        public List<QuestRewardObjectAssetModel> RewardObjectList { get; set; }

        /// <summary>
        /// Parent object.
        /// </summary>
        public long QuestId { get; set; }
        public QuestAssetModel Quest { get; set; }
    }
}