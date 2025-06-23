using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyMemberInfoPacket : PacketWriter
    {
        private const int PacketNumber = 2313;

        public PartyMemberInfoPacket(KeyValuePair<byte, CharacterModel> member)
        {
            Type(PacketNumber);
            WriteByte(member.Key);
            WriteInt(member.Value.Partner.CurrentType);
            WriteInt(member.Value.CurrentHp);
            WriteInt(member.Value.HP);
            WriteInt(member.Value.CurrentDs);
            WriteInt(member.Value.DS);
            WriteInt(member.Value.Partner.CurrentHp);
            WriteInt(member.Value.Partner.HP);
            WriteInt(member.Value.Partner.CurrentDs);
            WriteInt(member.Value.Partner.DS);
            WriteShort(member.Value.Level);
            WriteShort(member.Value.Partner.Level);
        }
        public PartyMemberInfoPacket(KeyValuePair<byte, CharacterModel> member, bool lider)
        {
            Type(PacketNumber);
            int key = member.Key;
            key += 1;

            WriteByte((byte)key);
            WriteInt(member.Value.Partner.CurrentType);
            WriteInt(member.Value.CurrentHp);
            WriteInt(member.Value.HP);
            WriteInt(member.Value.CurrentDs);
            WriteInt(member.Value.DS);
            WriteInt(member.Value.Partner.CurrentHp);
            WriteInt(member.Value.Partner.HP);
            WriteInt(member.Value.Partner.CurrentDs);
            WriteInt(member.Value.Partner.DS);
            WriteShort(member.Value.Level);
            WriteShort(member.Value.Partner.Level);
        }
    }
}