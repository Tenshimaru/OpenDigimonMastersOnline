namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class MonsterSkillAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Digimon type/model
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Skill id.
        /// </summary>
        public int SkillId { get; set; }
    }
}