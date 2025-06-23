namespace DigitalWorldOnline.Commons.Models.Character
{
    public sealed partial class CharacterXaiModel
    {
        /// <summary>
        /// Equips a new xai.
        /// </summary>
        public void EquipXai(int itemId, int xGauge, int xCrystals)
        {
            ItemId = itemId;
            XGauge = xGauge;
            XCrystals = (short)xCrystals;
        }


        /// <summary>
        /// Removes the equiped xai.
        /// </summary>
        public void RemoveXai()
        {
            ItemId = 0;
            XGauge = 0;
            XCrystals = 0;
        }
    }
}