using DigitalWorldOnline.Commons.Writers;
using System.Net.Sockets;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TamerSkillRequestPacket : PacketWriter
    {
        private const int PacketNumber = 1328;

      
        public TamerSkillRequestPacket(int SkillId,int BuffId, int DurationTS)
        {
            Type(PacketNumber);
            WriteInt(SkillId);
            WriteInt(BuffId);
            WriteInt(DurationTS);
        }
    }
}