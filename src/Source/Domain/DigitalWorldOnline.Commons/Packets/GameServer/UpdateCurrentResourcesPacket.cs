using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class UpdateCurrentResourcesPacket : PacketWriter
    {
        private const int PacketNumber = 1023;

        public UpdateCurrentResourcesPacket(int handler, short currentHp, short currentDs, short currentFatigue)
        {
            Type(PacketNumber);
            WriteUInt((uint)handler);
            WriteShort(currentHp);
            WriteShort(currentDs);
            WriteShort(currentFatigue);
        }
    }
}