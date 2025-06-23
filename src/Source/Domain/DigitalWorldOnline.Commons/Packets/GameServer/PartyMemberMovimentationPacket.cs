using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyMemberMovimentationPacket : PacketWriter
    {
        private const int PacketNumber = 2320;

        public PartyMemberMovimentationPacket(KeyValuePair<byte, CharacterModel> member)
        {
            Type(PacketNumber);
            WriteByte(member.Key);
            WriteInt(member.Value.Location.X);
            WriteInt(member.Value.Location.Y);
            WriteInt(member.Value.Partner.Location.X);
            WriteInt(member.Value.Partner.Location.Y);
        }
    }
}