using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ChangeTamerModelPacket : PacketWriter
    {
        private const int PacketNumber = 1314;

      
        public ChangeTamerModelPacket(int newModel, short itemSlot)
        {
            Type(PacketNumber);
            WriteInt(newModel);
            WriteShort(itemSlot);

        }
    }
}