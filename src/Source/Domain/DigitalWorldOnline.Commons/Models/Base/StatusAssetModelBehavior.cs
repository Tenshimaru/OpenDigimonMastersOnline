namespace DigitalWorldOnline.Commons.Models
{
    public partial class StatusAssetModel
    {
        /// <summary>
        /// Setups base values for all status.
        /// </summary>
        public void Initialize()
        {
            ASValue = 3500;
            ARValue = 150;
            ATValue = 10;
            BLValue = 10;
            CTValue = 550;
            DEValue = 5;
            EVValue = 6;
            HPValue = 160;
            HTValue = 15;
            MSValue = 600;
            WSValue = 400;
        }

        /// <summary>
        /// Update digimon AR value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetAR(int status) => ARValue = status;

        /// <summary>
        /// Update digimon AS value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetAS(int status) => ASValue = status;

        /// <summary>
        /// Update digimon AT value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetAT(int status) => ATValue = status;

        /// <summary>
        /// Update digimon BL value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetBL(int status) => BLValue = status;

        /// <summary>
        /// Update digimon CT value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetCT(int status) => CTValue = status;

        /// <summary>
        /// Update digimon DE value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetDE(int status) => DEValue = status;

        /// <summary>
        /// Update digimon DS value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetDS(int status) => DSValue = status;

        /// <summary>
        /// Update digimon EV value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetEV(int status) => EVValue = status;

        /// <summary>
        /// Update digimon HP value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetHP(int status) => HPValue = status;

        /// <summary>
        /// Update digimon HT value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetHT(int status) => HTValue = status;

        /// <summary>
        /// Update digimon MS value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetMS(int status) => MSValue = status;

        /// <summary>
        /// Update digimon WS value.
        /// </summary>
        /// <param name="status">New value</param>
        public void SetWS(int status) => WSValue = status;
    }
}