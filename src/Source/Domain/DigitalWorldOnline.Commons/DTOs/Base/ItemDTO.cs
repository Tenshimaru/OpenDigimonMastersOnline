namespace DigitalWorldOnline.Commons.DTOs.Base
{
    public sealed class ItemDTO
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Client reference to the target item.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Amount of the current item.
        /// </summary>
        public int Amount { get; set; }

        public int Duration { get; set; }
        public DateTime EndDate { get; set; }

        public bool FirstExpired { get; set; }

        /// <summary>
        /// Tamer shop selling price in bits.
        /// </summary>
        public long TamerShopSellPrice { get; set; }

        /// <summary>
        /// Attribute multiplier.
        /// </summary>
        public byte Power { get; set; }

        /// <summary>
        /// Available status reroll.
        /// </summary>
        public byte RerollLeft { get; set; }
        public byte FamilyType { get; set; }

        /// <summary>
        /// Item slot.
        /// </summary>
        public int Slot { get; set; }

        /// <summary>
        /// Status list for accessory related operations.
        /// </summary>
        public List<ItemAccessoryStatusDTO> AccessoryStatus { get; set; }

        public List<ItemSocketStatusDTO> SocketStatus { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        public ItemListDTO ItemList { get; set; }
        public long ItemListId { get; set; }
    }
}
