using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyMemberJoinPacket : PacketWriter
    {
        private const int PacketNumber = 2305;

        public PartyMemberJoinPacket(KeyValuePair<byte, CharacterModel> member)
        {
            Type(PacketNumber);
            WriteInt(member.Key);
            WriteInt(member.Value.Model.GetHashCode());
            WriteShort(member.Value.Level);
            WriteString(member.Value.Name);
            WriteInt(member.Value.Partner.BaseType);
            WriteShort(member.Value.Partner.Level);
            WriteString(member.Value.Partner.Name);
            WriteInt(member.Value.Location.MapId);
            WriteInt(member.Value.Channel);
            WriteInt(member.Value.GeneralHandler);
            WriteInt(member.Value.Partner.GeneralHandler);
        }

        public PartyMemberJoinPacket(KeyValuePair<byte, CharacterModel> member, CharacterModel character)
        {
            Type(PacketNumber);
            WriteInt(member.Key);
            WriteInt(member.Value.Model.GetHashCode());
            WriteShort(member.Value.Level);
            WriteString(member.Value.Name);
            WriteInt(member.Value.Partner.BaseType);
            WriteShort(member.Value.Partner.Level);
            WriteString(member.Value.Partner.Name);
            WriteInt(member.Value.Location.MapId);
            WriteInt(member.Value.Channel);
            if (character.Channel == member.Value.Channel &&
                character.Location.MapId == member.Value.Location.MapId)
            {
                WriteInt(member.Value.GeneralHandler);
                WriteInt(member.Value.Partner.GeneralHandler);
            }
            else
            {
                WriteInt(0);
                WriteInt(0);
            }
        }
    }
}