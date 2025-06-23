namespace DigitalWorldOnline.Commons.DTOs.Base
{
    public class StatusDTO
    {
        /// <summary>
        /// Total Attack Speed value.
        /// </summary>
        public int ASValue { get; set; }

        /// <summary>
        /// Total Attack Range value.
        /// </summary>
        public int ARValue { get; set; }

        /// <summary>
        /// Total Attack value.
        /// </summary>
        public int ATValue { get; set; }

        /// <summary>
        /// Total Block value.
        /// </summary>
        public int BLValue { get; set; }

        /// <summary>
        /// Total Critical value.
        /// </summary>
        public int CTValue { get; set; } //TODO: separar CR e CD

        /// <summary>
        /// Total Defense value.
        /// </summary>
        public int DEValue { get; set; }

        /// <summary>
        /// Total DigiSoul value.
        /// </summary>
        public int DSValue { get; set; }

        /// <summary>
        /// Total Evasion value.
        /// </summary>
        public int EVValue { get; set; }

        /// <summary>
        /// Total Health value.
        /// </summary>
        public int HPValue { get; set; }

        /// <summary>
        /// Total Hit Rate value.
        /// </summary>
        public int HTValue { get; set; }

        /// <summary>
        /// Total Run Speed value.
        /// </summary>
        public int MSValue { get; set; }

        /// <summary>
        /// Total Walk Speed value.
        /// </summary>
        public int WSValue { get; set; }
    }
}