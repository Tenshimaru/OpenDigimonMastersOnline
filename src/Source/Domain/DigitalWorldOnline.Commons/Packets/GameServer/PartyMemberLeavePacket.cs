using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyMemberLeavePacket : PacketWriter
    {
        private const int PacketNumber = 2307;

        public PartyMemberLeavePacket(byte partySlot)
        {
            Type(PacketNumber);
            WriteByte(partySlot);
        }
    }
}