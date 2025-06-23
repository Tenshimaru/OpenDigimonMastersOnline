using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyMemberKickPacket : PacketWriter
    {
        private const int PacketNumber = 2306;

        public PartyMemberKickPacket(byte partySlot)
        {
            Type(PacketNumber);
            WriteByte(partySlot);
        }
    }
}