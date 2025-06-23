namespace DigitalWorldOnline.Commons.Enums.ClientEnums
{
    public enum LoginFailReasonEnum
    {
        /// <summary>
        /// Username not found on database
        /// </summary>
        UserNotFound = 18,

        /// <summary>
        /// Account has been banned
        /// </summary>
        BannedAccount = 68,

        /// <summary>
        /// Wrong password for this account
        /// </summary>
        IncorrectPassword = 73
    }
}
