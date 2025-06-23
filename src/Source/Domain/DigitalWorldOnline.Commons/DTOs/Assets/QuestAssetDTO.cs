using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class QuestAssetDTO
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
        /// Quest supplys.
        /// </summary>
        public List<QuestSupplyAssetDTO> QuestSupplies { get; set; }

        /// <summary>
        /// Quest conditions.
        /// </summary>
        public List<QuestConditionAssetDTO> QuestConditions { get; set; }

        /// <summary>
        /// Quest goals.
        /// </summary>
        public List<QuestGoalAssetDTO> QuestGoals { get; set; }

        /// <summary>
        /// Quest rewards.
        /// </summary>
        public List<QuestRewardAssetDTO> QuestRewards { get; set; }
        
        /// <summary>
        /// Quest triggerable events.
        /// </summary>
        public List<QuestEventAssetDTO> QuestEvents { get; set; }
    }
}