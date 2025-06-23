namespace DigitalWorldOnline.Commons.Models
{
    public partial class StatusAssetModel
    {
        /// <summary>
        /// Total Attack Speed value.
        /// </summary>
        public int ASValue { get; private set; }

        /// <summary>
        /// Total Attack Range value.
        /// </summary>
        public int ARValue { get; private set; }

        /// <summary>
        /// Total Attack value.
        /// </summary>
        public int ATValue { get; private set; }

        /// <summary>
        /// Total Block value.
        /// </summary>
        public int BLValue { get; private set; }

        /// <summary>
        /// Total Critical value.
        /// </summary>
        public int CTValue { get; private set; }
        
        /// <summary>
        /// Total Critical Damage value.
        /// </summary>
        public int CDValue { get; private set; }

        /// <summary>
        /// Total Defense value.
        /// </summary>
        public int DEValue { get; private set; }

        /// <summary>
        /// Total DigiSoul value.
        /// </summary>
        public int DSValue { get; private set; }

        /// <summary>
        /// Total Evasion value.
        /// </summary>
        public int EVValue { get; private set; }

        /// <summary>
        /// Total Health value.
        /// </summary>
        public int HPValue { get; private set; }

        /// <summary>
        /// Total Hit Rate value.
        /// </summary>
        public int HTValue { get; private set; }

        /// <summary>
        /// Total Run Speed value.
        /// </summary>
        public int MSValue { get; private set; }

        /// <summary>
        /// Total Walk Speed value.
        /// </summary>
        public int WSValue { get; private set; }

        public StatusAssetModel() { }

        public StatusAssetModel(
            int asValue,
            int arValue,
            int atValue,
            int blValue,
            int ctValue,
            int cdValue,
            int deValue,
            int dsValue,
            int evValue,
            int hpValue,
            int htValue,
            int msValue,
            int wsValue
            )
        {
            ASValue = asValue;
            ARValue = arValue;
            ATValue = atValue;
            BLValue = blValue;
            CTValue = ctValue;
            CDValue = cdValue;
            DEValue = deValue;
            DSValue = dsValue;
            EVValue = evValue;
            HPValue = hpValue;
            HTValue = htValue;
            MSValue = msValue;
            WSValue = wsValue;
        }
    }
}