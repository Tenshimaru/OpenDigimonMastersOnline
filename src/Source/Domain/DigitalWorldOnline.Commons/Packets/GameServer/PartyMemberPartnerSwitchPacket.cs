using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyMemberPartnerSwitchPacket : PacketWriter
    {
        private const int PacketNumber = 2316;

        public PartyMemberPartnerSwitchPacket(KeyValuePair<byte, CharacterModel> member)
        {
            Type(PacketNumber);
            WriteByte(member.Key);
            WriteInt(member.Value.Partner.BaseType);
            WriteString(member.Value.Partner.Name);
            
            WriteShort((short)member.Value.Partner.CurrentHp);
            WriteShort((short)member.Value.Partner.HP);
            WriteShort((short)member.Value.Partner.CurrentDs);
            WriteShort((short)member.Value.Partner.DS);
        }
    }
}