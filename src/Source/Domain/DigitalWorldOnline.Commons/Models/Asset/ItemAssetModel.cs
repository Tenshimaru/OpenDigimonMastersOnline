using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public class ItemAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Client reference for the target item.
        /// </summary>
        public int ItemId { get; private set; }

        /// <summary>
        /// Item name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Item class.
        /// </summary>
        public int Class { get; private set; }

        /// <summary>
        /// Item type.
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Item type N (Exp and attributes).
        /// </summary>
        public int TypeN { get; private set; }

        /// <summary>
        /// Item ApplyValueMin (attributes sockets equipments).
        /// </summary>
        public short ApplyValueMin { get; private set; }

        /// <summary>
        /// Item ApplyValueMax (attributes sockets equipments).
        /// </summary>
        public short ApplyValueMax { get; private set; }

        /// <summary>
        /// Item ApplyValueMax (attributes sockets equipments).
        /// </summary>
        public short ApplyElement { get; private set; }

        /// <summary>
        /// Item section.
        /// </summary>
        public int Section { get; private set; }

        /// <summary>
        /// Item sell type.
        /// </summary>
        public int SellType { get; private set; }

        /// <summary>
        /// Item bound type.
        /// </summary>
        /// <remarks>0 - Not Bound</remarks>
        /// <remarks>1 - Bind when equipped</remarks>
        /// <remarks>2 - Always bound</remarks>
        public int BoundType { get; private set; }

        /// <summary>
        /// Item time usage.
        /// </summary>
        /// <remarks>0 - Infinity</remarks> 
        /// <remarks>1 - Unique use</remarks>
        /// <remarks>2 - Remove buff (?)</remarks>
        /// <remarks>3 - Remove buff and delete (?)</remarks>
        /// <remarks>4 - Timed buff and delete (?)</remarks>
        public int UseTimeType { get; private set; }

        /// <summary>
        /// Item skill code.
        /// </summary>
        public long SkillCode { get; private set; }

        /// <summary>
        /// Item min tamer level.
        /// </summary>
        public byte TamerMinLevel { get; private set; }

        /// <summary>
        /// Item max tamer level.
        /// </summary>
        public byte TamerMaxLevel { get; private set; }

        /// <summary>
        /// Item min digimon level.
        /// </summary>
        public byte DigimonMinLevel { get; private set; }

        /// <summary>
        /// Item max digimon level.
        /// </summary>
        public byte DigimonMaxLevel { get; private set; }

        /// <summary>
        /// Item sell price in bits.
        /// </summary>
        public long SellPrice { get; private set; }

        /// <summary>
        /// Item scan price in bits.
        /// </summary>
        public int ScanPrice { get; private set; }

        /// <summary>
        /// Item buy price in digicore.
        /// </summary>
        public int DigicorePrice { get; private set; }

        /// <summary>
        /// Item id for buying with items.
        /// </summary>
        public int EventPriceId { get; private set; }

        /// <summary>
        /// Amount for buying with items.
        /// </summary>
        public int EventPriceAmount { get; private set; }

        /// <summary>
        /// Item duration in minutes.
        /// </summary>
        public int UsageTimeMinutes { get; private set; }

        /// <summary>
        /// Max item stack.
        /// </summary>
        public short Overlap { get; private set; }

        /// <summary>
        /// Item target enumeration.
        /// </summary>
        public ItemConsumeTargetEnum Target { get; private set; }

        //TODO: Behavior + Summary
        public SkillCodeAssetModel? SkillInfo { get; private set; }
        public int TimeInSeconds => (UsageTimeMinutes * 60) + 10;
        public bool TemporaryItem => UsageTimeMinutes > 0;
        public void SetSkillInfo(SkillCodeAssetModel? skillCode) => SkillInfo ??= skillCode;
    }
}