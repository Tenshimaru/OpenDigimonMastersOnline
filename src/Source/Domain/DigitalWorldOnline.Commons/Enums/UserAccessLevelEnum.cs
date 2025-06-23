namespace DigitalWorldOnline.Commons.Enums
{
    public enum UserAccessLevelEnum
    {
        /// <summary>
        /// Unauthorized access.
        /// </summary>
        Unauthorized = 0,
        
        /// <summary>
        /// Default access.
        /// </summary>
        Default = 5,

        /// <summary>
        /// Game master access.
        /// </summary>
        GameMaster = 10,

        /// <summary>
        /// Administrator access.
        /// </summary>
        Administrator = 15
    }
}