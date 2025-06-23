using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class AvailableChannelsPacket : PacketWriter
    {
        private const int PacketNumber = 1713;

        /// <summary>
        /// Sends the current available channels list.
        /// </summary>
        public AvailableChannelsPacket(Dictionary<byte, byte> channels)
        {
            Type(PacketNumber);

            foreach (var channel in channels)
            {
                WriteByte(channel.Key);
                WriteByte(UtilitiesFunctions.GetChannelLoad(channel.Value));
            }
            WriteByte(byte.MaxValue);
        }

        //public IDictionary<byte, byte> Channels { get; }
    }
}