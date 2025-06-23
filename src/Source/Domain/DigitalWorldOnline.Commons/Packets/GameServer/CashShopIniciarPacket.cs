using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class CashShopIniciarPacket : PacketWriter
    {
        private const int PacketNumber = 3412;

        /// <summary>
        /// Load the Cash Shop
        /// </summary>
        /// <param name="remainingSeconds">The membership remaining seconds (UTC).</param>
        public CashShopIniciarPacket()
        {
            Type(PacketNumber);
            WriteInt(0);
        }
    }
}