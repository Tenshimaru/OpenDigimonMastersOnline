using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class PickItemFailPacket : PacketWriter
    {
        private const int PacketNumber = 3913;

        /// <summary>
        /// Fail on item pick from the ground.
        /// </summary>
        /// <param name="failReason">The tamer appearance handler.</param>
        public PickItemFailPacket(PickItemFailReasonEnum failReason)
        {
            Type(PacketNumber);
            WriteInt(failReason.GetHashCode());
        }
    }
}