using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;

namespace DigitalWorldOnline.Commons.Models.Base
{
    public partial class ItemListModel
    {
        /// <summary>
        /// Unique sequential identifier.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Item list enumeration.
        /// </summary>
        public ItemListEnum Type { get; private set; }

        /// <summary>
        /// Current item list slots amount.
        /// </summary>
        public byte Size { get; private set; }

        /// <summary>
        /// Item list bits.
        /// </summary>
        public long Bits { get; private set; }

        /// <summary>
        /// Items inside the list.
        /// </summary>
        public List<ItemModel> Items { get; private set; }

        public List<ItemModel> EquippedItems => Items.Where(x => x.ItemId > 0).ToList();

        public ItemListModel(ItemListEnum type)
        {
            Items = new List<ItemModel>();

            Type = type;

            switch (Type)
            {
                case ItemListEnum.Equipment:
                    Size = (byte)GeneralSizeEnum.Equipment;
                    break;

                case ItemListEnum.Inventory:
                    Size = (byte)GeneralSizeEnum.InitialInventory;
                    break;

                case ItemListEnum.Warehouse:
                    Size = (byte)GeneralSizeEnum.InitialWarehouse;
                    break;

                case ItemListEnum.Chipsets:
                    Size = (byte)GeneralSizeEnum.Chipsets;
                    break;

                case ItemListEnum.JogressChipset:
                    Size = (byte)GeneralSizeEnum.JogressChipset;
                    break;

                case ItemListEnum.Digivice:
                    Size = (byte)GeneralSizeEnum.Digivice;
                    break;

                case ItemListEnum.TamerSkill:
                    Size = (byte)GeneralSizeEnum.TamerSkill;
                    break;

                case ItemListEnum.RewardWarehouse:
                    Size = (byte)GeneralSizeEnum.RewardWarehouse;
                    break;

                case ItemListEnum.GiftWarehouse:
                    Size = (byte)GeneralSizeEnum.GiftWarehouse;
                    break;

                case ItemListEnum.ShopWarehouse:
                    Size = (byte)GeneralSizeEnum.ShopWarehouse;
                    break;

                case ItemListEnum.AccountWarehouse:
                    Size = (byte)GeneralSizeEnum.InitialAccountWarehouse;
                    break;

                case ItemListEnum.CashWarehouse:
                    Size = (byte)GeneralSizeEnum.CashWarehouse;
                    break;

                case ItemListEnum.BuyHistory:
                    Size = (byte)GeneralSizeEnum.CashShopBuyHistory;
                    break;

                case ItemListEnum.TamerShop:
                    Size = (byte)GeneralSizeEnum.PersonalShop;
                    break;

                case ItemListEnum.ConsignedShop:
                    Size = (byte)GeneralSizeEnum.ConsignedShop;
                    break;

                case ItemListEnum.ConsignedWarehouse:
                    Size = (byte)GeneralSizeEnum.ConsignedShopWarehouse;
                    break;

                case ItemListEnum.TradeItems:
                    Size = (byte)GeneralSizeEnum.TradeItems;
                    break;
            }

            for (var i = 0; i < Size; i++)
                Items.Add(new ItemModel(Items.Any() ? Items.Max(x => x.Slot) : -1));
        }
    }
}