using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Asset
{
    public sealed class BuffInfoAssetModel
    {
        public long Id { get; private set; }
        public int BuffId { get; private set; }
        public string Name { get; private set; }
        public int DigimonSkillCode { get; private set; }
        public int SkillCode { get; private set; }
        public int MinLevel { get; private set; }
        public int ConditionLevel { get; private set; }
        public short Class { get; private set; }
        public int Type { get; private set; }
        public int LifeType { get; private set; }
        public int TimeType { get; private set; }

        //TODO: Behavior
        public SkillCodeAssetModel? SkillInfo { get; set; }
        public int SkillId => DigimonSkillCode > 0 ? DigimonSkillCode : SkillCode;
        public short EffectValue => (short)(SkillInfo.Apply.First().Value / 5);
        public void SetSkillInfo(SkillCodeAssetModel? skillCode) => SkillInfo ??= skillCode;

        public bool Pray => (SkillPrayType)DigimonSkillCode == SkillPrayType.Normal
            || (SkillPrayType)DigimonSkillCode == SkillPrayType.Normal1 ||
            (SkillPrayType)DigimonSkillCode == SkillPrayType.Ultimate
            ||
            (SkillPrayType)SkillCode == SkillPrayType.Normal
            || (SkillPrayType)SkillCode == SkillPrayType.Normal1 ||
            (SkillPrayType)SkillCode == SkillPrayType.Ultimate;

        public bool Cheer => (SkillCheerType)DigimonSkillCode == SkillCheerType.Normal
            ||
            (SkillCheerType)DigimonSkillCode == SkillCheerType.Normal1
            || (SkillCheerType)DigimonSkillCode == SkillCheerType.Normal2 ||
            (SkillCheerType)DigimonSkillCode == SkillCheerType.Ultimate || 
            (SkillCheerType)SkillCode == SkillCheerType.Normal
            ||
            (SkillCheerType)SkillCode == SkillCheerType.Normal1
            || (SkillCheerType)SkillCode == SkillCheerType.Normal2 ||
            (SkillCheerType)SkillCode == SkillCheerType.Ultimate;
    }
}