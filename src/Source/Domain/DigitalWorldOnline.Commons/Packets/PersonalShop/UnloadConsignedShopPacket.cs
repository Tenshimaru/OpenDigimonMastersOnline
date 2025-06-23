using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.PersonalShop
{
    public class UnloadConsignedShopPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Hides the target consigned shop on the map.
        /// </summary>
        /// <param name="consignedShop">The consigned shop to be loaded.</param>
        public UnloadConsignedShopPacket(ConsignedShop consignedShop)
        {
            Type(PacketNumber);
            WriteByte(2);
            WriteShort(1);
            WriteInt64(consignedShop.GeneralHandler);
        }

        /// <summary>
        /// Hides the target consigned shop on the map.
        /// </summary>
        /// <param name="handler">The target consigned shop handler.</param>
        public UnloadConsignedShopPacket(long handler)
        {
            Type(PacketNumber);
            WriteByte(2);
            WriteShort(1);
            WriteInt64(handler);
        }

        /// <summary>
        /// Hides the consigned shop list on the map.
        /// </summary>
        /// <param name="consignedShopList">The consigned shop list to be unloaded.</param>
        public UnloadConsignedShopPacket(IList<ConsignedShop> consignedShopList)
        {
            Type(PacketNumber);
            WriteByte(2);
            WriteShort((short)consignedShopList.Count);

            foreach (var consignedShop in consignedShopList)
                WriteInt64(consignedShop.GeneralHandler);

            WriteInt(0);
        }
    }
}