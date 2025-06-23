using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyLeaderChangedPacket : PacketWriter
    {
        private const int PacketNumber = 2308;

        public PartyLeaderChangedPacket(int newLeaderSlot)
        {
            Type(PacketNumber);
            WriteInt(newLeaderSlot);
        }
    }
}