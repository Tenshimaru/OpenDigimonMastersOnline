namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class MonsterSkillAssetModel
    {
        /// <summary>
        /// Unique sequential identifier
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Digimon type/model
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Skill id.
        /// </summary>
        public int SkillId { get; set; }

        /// <summary>
        /// Detailed skill information.
        /// </summary>
        public MonsterSkillInfoAssetModel SkillInfo { get; private set; }

        //TODO: Behavior
        public void SetSkillInfo(MonsterSkillInfoAssetModel skillInfo) => SkillInfo ??= skillInfo;
    }
}