
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ItemSocketInPacket : PacketWriter
    {
        private const int PacketNumber = 3926;


        public ItemSocketInPacket( int Money)
        {
            Type(PacketNumber);
            WriteInt(100);
            WriteInt(Money);
            WriteInt(0);
         
        }
    }
}
