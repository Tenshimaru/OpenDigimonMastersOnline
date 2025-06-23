namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class SkillCodeAssetModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Client reference for the skill code.
        /// </summary>
        public long SkillCode { get; private set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// Applicable attributes.
        /// </summary>
        public List<SkillCodeApplyAssetModel> Apply { get; private set; }
    }
}
