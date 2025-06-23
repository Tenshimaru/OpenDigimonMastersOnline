using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.PersonalShop
{
    public class ConsignedShopItemsViewPacket : PacketWriter
    {
        private const int PacketNumber = 1520;

        /// <summary>
        /// Shows the selling item list in the target consigned shop.
        /// </summary>
        /// <param name="consignedShop">The consigned shop basic information.</param>
        /// <param name="consignedShopItems">The consigned shop items.</param>
        /// <param name="ownerName">The consigned shop owner name.</param>
        public ConsignedShopItemsViewPacket(ConsignedShop consignedShop, ItemListModel consignedShopItems, string ownerName, bool isOwner = false)
        {
            Type(PacketNumber);
            WriteInt(100);
            WriteInt(consignedShop.ItemId);
            WriteString(consignedShop.ShopName);
            WriteString(ownerName);
            
            WriteInt(consignedShopItems.Count);
            foreach (var item in consignedShopItems.Items.Where(x => x.ItemId > 0))
            {
                WriteBytes(item.ToArray());
                WriteInt64(item.TamerShopSellPrice);
            }

            WriteInt(consignedShopItems.Count);
            foreach (var item in consignedShopItems.Items.Where(x => x.ItemId > 0))
            {
                WriteBytes(item.ToArray());
                WriteInt64(item.TamerShopSellPrice);
            }

            WriteByte(isOwner ? (byte)1 : (byte)0);
        }

        public ConsignedShopItemsViewPacket()
        {
            Type(PacketNumber);
            WriteInt(0);
        }
    }
}