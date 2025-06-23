namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class TamerSkillAssetDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; set; }

        public int SkillId { get; set; }

        public int SkillCode { get; set; }

        public int Duration { get; set; }

        public int Type { get; set; }

        public int BuffId { get; set; }
    }
}