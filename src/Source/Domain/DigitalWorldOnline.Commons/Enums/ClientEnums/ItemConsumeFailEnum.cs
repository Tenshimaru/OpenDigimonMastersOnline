namespace DigitalWorldOnline.Commons.Enums.ClientEnums
{
    public enum ItemConsumeFailEnum
    {
        Expired = 1,
        InBattle,
        InCooldown,
        InvalidArea,
        NoTarget,
        InvalidTarget,
        OutOfRange,
        UnavailableTarget,
        ConditionNotMet,
        UseLimitReached,

        CantBeApplyed = 12,
        ScannableItem,
        NotUsingOneSpiritDigimon,
        MaxLimit,
        NotUnlocked,
        OtherError
    }
}
