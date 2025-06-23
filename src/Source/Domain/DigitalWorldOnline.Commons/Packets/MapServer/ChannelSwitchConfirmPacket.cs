using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.MapServer
{
    public class ChannelSwitchConfirmPacket : PacketWriter
    {
        private const int PacketNumber = 1703;

        /// <summary>
        /// Sends the channel switch confirmation.
        /// </summary>
        public ChannelSwitchConfirmPacket()
        {
            Type(PacketNumber);
        }
    }
}