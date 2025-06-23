using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ChangeSpecificStatusPacket : PacketWriter
    {
        private const int PacketNumber = 1084;

        /// <summary>
        /// Changes the target status.
        /// </summary>
        /// <param name="handler">The target handler</param>
        /// <param name="status">The desired status enum</param>
        /// <param name="newValue">The status new value</param>
        public ChangeSpecificStatusPacket(int handler, ApplyStatusEnum status, int newValue)
        {
            Type(PacketNumber);
            WriteInt(handler);
            WriteShort(1);
            WriteShort((short)status);
            WriteInt(newValue);
        }
    }
}