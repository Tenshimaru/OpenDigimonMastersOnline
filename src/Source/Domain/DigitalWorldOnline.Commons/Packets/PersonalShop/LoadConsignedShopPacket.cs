using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.PersonalShop
{
    public class LoadConsignedShopPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Shows the target consigned shop on the map.
        /// </summary>
        /// <param name="consignedShop">The consigned shop to be showed.</param>
        public LoadConsignedShopPacket(ConsignedShop consignedShop)
        {
            Type(PacketNumber);
            WriteByte(3);
            WriteShort(1);
            WriteInt(consignedShop.Location.X);
            WriteInt(consignedShop.Location.Y);
            WriteUInt(consignedShop.GeneralHandler);
            WriteFloat(0);
            WriteInt(0);
            WriteInt(consignedShop.ItemId);
            WriteString(consignedShop.ShopName);
            WriteByte(0);
        }

        /// <summary>
        /// Shows the consigned shop list on the map.
        /// </summary>
        /// <param name="consignedShopList">The consigned shop list to be loaded.</param>
        public LoadConsignedShopPacket(IList<ConsignedShop> consignedShopList)
        {
            Type(PacketNumber);
            WriteByte(3);
            WriteShort((short)consignedShopList.Count);
            foreach (var consignedShop in consignedShopList)
            {
                WriteInt(consignedShop.Location.X);
                WriteInt(consignedShop.Location.Y);
                WriteInt64(consignedShop.GeneralHandler);
                WriteShort((short)consignedShop.Location.X);
                WriteShort((short)consignedShop.Location.Y);
                WriteInt(consignedShop.ItemId);
                WriteString(consignedShop.ShopName);
            }
            WriteInt(0);
        }
    }
}