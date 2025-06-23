using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class ReceiveGiftRewardItemFailPacket : PacketWriter
    {
        private const int PacketNumber = 3944;

        public ReceiveGiftRewardItemFailPacket(ReceiveGiftRewardItemFailReasonEnum failReason)
        {
            Type(PacketNumber);
            WriteInt(failReason.GetHashCode());
        }
    }
}