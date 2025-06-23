using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class QuestAssetModel
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client reference for quest.
        /// </summary>
        public int QuestId { get; set; }

        /// <summary>
        /// Quest type enumeration.
        /// </summary>
        public QuestTypeEnum QuestType { get; set; }

        /// <summary>
        /// Quest target type.
        /// </summary>
        public int TargetType { get; set; }

        /// <summary>
        /// Quest target identifier.
        /// </summary>
        public int TargetValue { get; set; }

        /// <summary>
        /// Quest target identifier.
        /// </summary>
        public int unlockedLevel { get; set; }

        /// <summary>
        /// Quest supplys.
        /// </summary>
        public List<QuestSupplyAssetModel> QuestSupplies { get; set; }

        /// <summary>
        /// Quest conditions.
        /// </summary>
        public List<QuestConditionAssetModel> QuestConditions { get; set; }

        /// <summary>
        /// Quest goals.
        /// </summary>
        public List<QuestGoalAssetModel> QuestGoals { get; set; }

        /// <summary>
        /// Quest rewards.
        /// </summary>
        public List<QuestRewardAssetModel> QuestRewards { get; set; }

        /// <summary>
        /// Quest triggerable events.
        /// </summary>
        public List<QuestEventAssetModel> QuestEvents { get; set; }
    }
}