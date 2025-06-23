namespace DigitalWorldOnline.Commons.Enums.Account
{
    public enum AccountBlockEnum
    {
        /// <summary>
        /// Unblocked account.
        /// </summary>
        Unblocked = 0,

        /// <summary>
        /// Account blocked for a short period of time.
        /// </summary>
        Short = 1,

        /// <summary>
        /// Account blocked for a medium period of time.
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Account blocked for a long period of time.
        /// </summary>
        Long = 3,

        /// <summary>
        /// Account permanently blocked.
        /// </summary>
        Permanent = 4
    }
}
