using DigitalWorldOnline.Commons.Models.Asset;
using System.Text.Json.Serialization;

namespace DigitalWorldOnline.Commons.Models.Base
{
    public partial class ItemModel : ICloneable
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

        public bool ExpiredItem { get; set; }

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

        public int TradeSlot { get; set; }

        /// <summary>
        /// Status list for accessory related operations.
        /// </summary>
        public List<ItemAccessoryStatusModel> AccessoryStatus { get; set; }

        /// <summary>
        /// Socket list for accessory related operations.
        /// </summary>
        public List<ItemSocketStatusModel> SocketStatus { get; set; }

        /// <summary>
        /// JIT property.
        /// </summary>
        public ItemAssetModel? ItemInfo { get; set; }

        /// <summary>
        /// Reference to the owner.
        /// </summary>
        [JsonIgnore]
        public ItemListModel ItemList { get; set; }
        public long ItemListId { get; set; }

        public ItemModel(int lastSlot)
        {
            Id = Guid.NewGuid();

            Slot = lastSlot + 1;

            if (AccessoryStatus == null || AccessoryStatus.Count != 8)
            {
                AccessoryStatus = new List<ItemAccessoryStatusModel>()
                {
                    new ItemAccessoryStatusModel(0),
                    new ItemAccessoryStatusModel(1),
                    new ItemAccessoryStatusModel(2),
                    new ItemAccessoryStatusModel(3),
                    new ItemAccessoryStatusModel(4),
                    new ItemAccessoryStatusModel(5),
                    new ItemAccessoryStatusModel(6),
                    new ItemAccessoryStatusModel(7)
                };
            }

            if (SocketStatus == null || SocketStatus.Count != 3)
            {
                SocketStatus = new List<ItemSocketStatusModel>()
                {
                    new ItemSocketStatusModel(0),
                    new ItemSocketStatusModel(1),
                    new ItemSocketStatusModel(2),

                };
            }
        }


        public ItemModel()
        {
            Id = Guid.NewGuid();
            FirstExpired = true;

            if (AccessoryStatus == null || AccessoryStatus.Count != 8)
            {
                AccessoryStatus = new List<ItemAccessoryStatusModel>()
                {
                    new ItemAccessoryStatusModel(0),
                    new ItemAccessoryStatusModel(1),
                    new ItemAccessoryStatusModel(2),
                    new ItemAccessoryStatusModel(3),
                    new ItemAccessoryStatusModel(4),
                    new ItemAccessoryStatusModel(5),
                    new ItemAccessoryStatusModel(6),
                    new ItemAccessoryStatusModel(7)
                };
            }

            if (SocketStatus == null || SocketStatus.Count != 3)
            {
                SocketStatus = new List<ItemSocketStatusModel>()
                {
                    new ItemSocketStatusModel(0),
                    new ItemSocketStatusModel(1),
                    new ItemSocketStatusModel(2)

                };
            }
        }
    }
}