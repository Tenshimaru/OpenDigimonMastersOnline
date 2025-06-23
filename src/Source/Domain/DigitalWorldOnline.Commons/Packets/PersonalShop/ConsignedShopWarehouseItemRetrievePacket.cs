using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.PersonalShop
{
    public class ConsignedShopWarehouseItemRetrievePacket : PacketWriter
    {
        private const int PacketNumber = 1521;

        /// <summary>
        /// Answer with consigned shop warehouse item withdraw.
        /// </summary>
        public ConsignedShopWarehouseItemRetrievePacket()
        {
            Type(PacketNumber);
            WriteInt(0);
        }
    }
}