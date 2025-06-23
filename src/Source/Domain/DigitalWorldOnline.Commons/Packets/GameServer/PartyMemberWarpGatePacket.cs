using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyMemberWarpGatePacket : PacketWriter
    {
        private const int PacketNumber = 2315;

        public PartyMemberWarpGatePacket(KeyValuePair<byte, CharacterModel> member, CharacterModel character)
        {
            Type(PacketNumber);
            WriteByte(member.Key);
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