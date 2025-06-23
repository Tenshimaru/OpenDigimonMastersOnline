namespace DigitalWorldOnline.Commons.Models.Config
{
    public sealed partial class ItemDropConfigModel
    {
        /// <summary>
        /// Updates the config name.
        /// </summary>
        /// <param name="name">New name</param>
        public void SetName(string name) => Name = name;
        
        /// <summary>
        /// Updates the config item id.
        /// </summary>
        /// <param name="itemId">New item id</param>
        public void SetItemId(int itemId) => ItemId = itemId;
        
        /// <summary>
        /// Updates the min drop quantity.
        /// </summary>
        /// <param name="amount">The new amount</param>
        public void SetMinAmount(int amount) => MinAmount = amount;
        
        /// <summary>
        /// Updates the max drop quantity.
        /// </summary>
        /// <param name="amount">The new amount</param>
        public void SetMaxAmount(int amount) => MaxAmount = amount;
        
        /// <summary>
        /// Updates the drop chance.
        /// </summary>
        /// <param name="chance">The new drop chance</param>
        public void SetChance(double chance) => Chance = chance;
    }
}