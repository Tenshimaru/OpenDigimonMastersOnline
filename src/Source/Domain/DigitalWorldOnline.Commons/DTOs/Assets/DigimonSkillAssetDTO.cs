namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class DigimonSkillAssetDTO
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
        /// Skill slot (1=F1, 2=F2... x=Fx)
        /// </summary>
        public byte Slot { get; set; }

        /// <summary>
        /// Skill id.
        /// </summary>
        public int SkillId { get; set; }
    }
}