namespace DigitalWorldOnline.Commons.Enums.ClientEnums
{
    public enum SecondaryPasswordCheckEnum
    {
        /// <summary>
        /// When the user is performing a common access.
        /// </summary>
        Check = 2,

        /// <summary>
        /// When the user skips the input screen.
        /// </summary>
        DontCheck = 3,

        /// <summary>
        /// User skipped or entered correct password on input screen.
        /// </summary>
        CorrectOrSkipped = 0,

        /// <summary>
        /// User entered incorrect password on input screen or skipped it.
        /// </summary>
        Incorrect = 20052
    }
}
