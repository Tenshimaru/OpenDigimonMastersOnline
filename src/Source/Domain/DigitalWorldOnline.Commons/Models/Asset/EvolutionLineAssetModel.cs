namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class EvolutionLineAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long EvolutionId { get; private set; }

        /// <summary>
        /// Base type for the evolution.
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Required slot level to unlock.
        /// </summary>
        public byte SlotLevel { get; private set; }

        /// <summary>
        /// Required partner level to unlock.
        /// </summary>
        public byte UnlockLevel { get; private set; }

        /// <summary>
        /// Required quest to unlock.
        /// </summary>
        public short UnlockQuestId { get; private set; }

        /// <summary>
        /// Required item to unlock.
        /// </summary>
        public int UnlockItemSection { get; private set; }

        /// <summary>
        /// Required item amount to unlock.
        /// </summary>
        public int UnlockItemSectionAmount { get; private set; }

        public int RequiredItem { get; private set; }

        public int RequiredAmount { get; private set; }

        /// <summary>
        /// Available stages.
        /// </summary>
        public List<EvolutionStageAssetModel> Stages { get; private set; }
    }
}