using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class CashShopCoinsPacket : PacketWriter
    {
        private const int PacketNumber = 3404;

       /// <summary>
       /// <param name="premium"></param>
       /// <param name="silk"></param>
       /// </summary>
        public CashShopCoinsPacket(int premium, int silk)
        {
            Type(PacketNumber);
            WriteInt(0);
            WriteInt(silk);
            WriteInt(premium);
        }
    }
}