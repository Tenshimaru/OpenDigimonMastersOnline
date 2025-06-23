using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.PersonalShop
{
    public class LoadConsignedShopWarehousePacket : PacketWriter
    {
        private const int PacketNumber = 1523;

        /// <summary>
        /// Pop up the personal shop window.
        /// </summary>
        /// <param name="shopAction">The target action enum to choose the shop type</param>
        /// <param name="itemId">The item id used to open the shop</param>
        public LoadConsignedShopWarehousePacket(ItemListModel consignedWarehouse)
        {
            Type(PacketNumber);
            WriteInt(consignedWarehouse.RetrieveEnabled);
            WriteInt64(consignedWarehouse.Bits);
            WriteUInt(consignedWarehouse.Count);

            foreach (var item in consignedWarehouse.Items.Where(x => x.ItemId > 0))
            {
                WriteBytes(item.ToArray());
            }
        }
    }
}