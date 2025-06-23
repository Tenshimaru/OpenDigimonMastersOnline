using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class SkillCodeApplyAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The apply type for the target skill code.
        /// </summary>
        public SkillCodeApplyTypeEnum Type { get; set; }

        /// <summary>
        /// The attribute type for target skill code.
        /// </summary>
        public SkillCodeApplyAttributeEnum Attribute { get; set; }

        /// <summary>
        /// Attribute base value.
        /// </summary>
        public int Value { get; set; }
        public int Chance { get; set; }

        /// <summary>
        /// Attribute aditional value.
        /// </summary>
        public int AdditionalValue { get; set; }

        /// <summary>
        /// Attribute Increase value.
        /// </summary>
        public int IncreaseValue { get; set; }

      
    }
}
