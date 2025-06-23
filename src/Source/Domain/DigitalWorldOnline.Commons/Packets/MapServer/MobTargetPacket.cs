using DigitalWorldOnline.Commons.Writers;
using System.Net.Sockets;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class MobTargetPacket : PacketWriter
    {
        private const int PacketNumber = 1017;

        public MobTargetPacket(int Handle, byte MonsterLevel, byte HpRate, int TamerHandle, int StartTime, int EndTime)
        {
            Type(PacketNumber);
            WriteInt(Handle);
            WriteByte(HpRate);
            WriteInt(TamerHandle);
            WriteInt(EndTime);
            WriteInt(StartTime);
        }
    }
}