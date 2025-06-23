using DigitalWorldOnline.Commons.Enums.Party;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyCreatedPacket : PacketWriter
    {
        private const int PacketNumber = 2319;

        public PartyCreatedPacket(int partyId, PartyLootShareTypeEnum lootType)
        {
            Type(PacketNumber);
            WriteInt(partyId);
            WriteInt((int)lootType);
        }
    }
}