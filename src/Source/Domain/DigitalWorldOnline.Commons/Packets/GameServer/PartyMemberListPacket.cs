using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyMemberListPacket : PacketWriter
    {
        private const int PacketNumber = 2310;

        public PartyMemberListPacket(GameParty party, long memberId, byte slots = 1)
        {
            Type(PacketNumber);

            WriteUInt((uint)party.Id);
            WriteInt(party[memberId].Key);
            WriteInt(party.LeaderSlot);

            WriteByte((byte)party.LootType);
            WriteByte((byte)party.LootFilter);

            WriteByte(slots);

            foreach (var member in party.Members.Where(x => x.Value.Id != memberId))
            {
                WriteInt(member.Key);

                WriteInt(member.Value.Model.GetHashCode());
                WriteShort(member.Value.Level);
                WriteString(member.Value.Name);

                WriteInt(member.Value.Partner.CurrentType);
                WriteShort(member.Value.Partner.Level);
                WriteString(member.Value.Partner.Name);

                WriteInt(member.Value.Location.MapId);
                WriteInt(member.Value.Channel);
                if (party[memberId].Value.Channel == member.Value.Channel &&
                    party[memberId].Value.Location.MapId == member.Value.Location.MapId)
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

            WriteInt(-1);
        }
    }
}