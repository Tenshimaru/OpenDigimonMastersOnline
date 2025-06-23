namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class EvolutionLineAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Base type for the evolution.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Required slot level to unlock.
        /// </summary>
        public byte SlotLevel { get; private set; }

        /// <summary>
        /// Required partner level to unlock.
        /// </summary>
        public byte UnlockLevel { get; set; }

        /// <summary>
        /// Required quest to unlock.
        /// </summary>
        public short UnlockQuestId { get; set; }

        /// <summary>
        /// Required item to unlock.
        /// </summary>
        public int UnlockItemSection { get; set; }

        /// <summary>
        /// Required item amount to unlock.
        /// </summary>
        public int UnlockItemSectionAmount { get; set; }

        public int RequiredItem { get; private set; }

        public int RequiredAmount { get; private set; }

        /// <summary>
        /// Available stages.
        /// </summary>
        public List<EvolutionStageAssetDTO> Stages { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public long EvolutionId { get; set; }
        public EvolutionAssetDTO Evolution { get; set; }
    }
}