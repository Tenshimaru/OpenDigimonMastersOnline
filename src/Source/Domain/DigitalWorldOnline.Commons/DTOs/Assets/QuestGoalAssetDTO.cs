using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class QuestGoalAssetDTO
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Quest goal type enumeration.
        /// </summary>
        public QuestGoalTypeEnum GoalType { get; set; }

        /// <summary>
        /// Quest goal identifier.
        /// </summary>
        public int GoalId { get; set; }

        /// <summary>
        /// Quest goal amount.
        /// </summary>
        public int GoalAmount { get; set; }

        /// <summary>
        /// Quest CurType amount. (?)
        /// </summary>
        public int CurTypeCount { get; set; }

        /// <summary>
        /// Quest subvalue. (?)
        /// </summary>
        public int SubValue { get; set; }

        /// <summary>
        /// Quest second subvalue. (?)
        /// </summary>
        public int SubValueTwo { get; set; }

        /// <summary>
        /// Parent object.
        /// </summary>
        public long QuestId { get; set; }
        public QuestAssetDTO Quest { get; set; }
    }
}