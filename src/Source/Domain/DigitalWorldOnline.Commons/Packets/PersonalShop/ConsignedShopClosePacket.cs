using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.PersonalShop
{
    public class ConsignedShopClosePacket : PacketWriter
    {
        private const int PacketNumber = 1517;

        /// <summary>
        /// Flags the openned consigned shop as closed.
        /// </summary>
        public ConsignedShopClosePacket()
        {
            Type(PacketNumber);
            WriteInt(100);
        }
    }
}