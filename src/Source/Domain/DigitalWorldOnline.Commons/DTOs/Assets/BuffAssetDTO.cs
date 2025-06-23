namespace DigitalWorldOnline.Commons.DTOs.Assets
{
    public sealed class BuffAssetDTO
    {
        public long Id { get; set; }
        public int BuffId { get; set; }
        public string Name { get; set; }
        public int DigimonSkillCode { get; set; }
        public int SkillCode { get; set; }
        public int MinLevel { get; set; }
        public int ConditionLevel { get; set; }
        public short Class { get; set; }
        public int Type { get; set; }
        public int LifeType { get; set; }
        public int TimeType { get; set; }
    }
}