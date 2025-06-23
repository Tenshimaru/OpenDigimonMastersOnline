namespace DigitalWorldOnline.Commons.Models.Base
{
    public class ItemDrop
    {
        /// <summary>
        /// Sequencial unique identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Client item id information.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Amount of the item.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Chance of drop.
        /// </summary>
        public byte Chance { get; set; }

        /// <summary>
        /// Remaining duration of the item in unix total seconds.
        /// </summary>
        public int RemainingTime { get; set; }

        /// <summary>
        /// Reference to the drop reward.
        /// </summary>
        public long DropRewardId { get; set; }

        //TODO: description
        public int X { get; private set; }
        public int Y { get; private set; }
        public uint Handle { get; private set; }
    }
}
