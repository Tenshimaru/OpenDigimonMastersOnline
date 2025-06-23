namespace DigitalWorldOnline.Commons.Enums.ClientEnums
{
    public enum SecondaryPasswordScreenEnum
    {
        /// <summary>
        /// Hides the secondary password screen after login.
        /// </summary>
        Hide = 1,

        /// <summary>
        /// Requests the user to type the secondary password for access.
        /// </summary>
        RequestInput = 2,

        /// <summary>
        /// Requests the user to setup the secondary password for first use.
        /// </summary>
        RequestSetup = 3
    }
}
