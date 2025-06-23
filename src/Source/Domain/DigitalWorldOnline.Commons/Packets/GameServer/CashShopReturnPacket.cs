using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class CashShopReturnPacket : PacketWriter
    {
        private const int PacketNumber = 3413;

        /// <summary>
        /// Confirmed Cash Shop
        /// </summary>
        /// <param name="remainingSeconds">The membership remaining seconds (UTC).</param>
        public CashShopReturnPacket(short Result, int RealCash, int RealBonus, sbyte TotalSuccess, sbyte TotalFail)
        {
            Type(PacketNumber);
            WriteShort(Result);
            WriteInt(RealCash);
            WriteInt(RealBonus);

        }
    }
}