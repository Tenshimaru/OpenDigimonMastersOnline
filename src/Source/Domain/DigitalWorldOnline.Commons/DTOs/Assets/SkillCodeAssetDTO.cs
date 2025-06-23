namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class SkillCodeAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Client reference for the skill code.
        /// </summary>
        public long SkillCode { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Applicable attributes.
        /// </summary>
        public IList<SkillCodeApplyAssetDTO> Apply { get; set; }
    }
}
