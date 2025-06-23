using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DisconnectUserPacket : PacketWriter
    {
        private const int PacketNumber = 9923;

        /// <summary>
        /// Disconnects the target client.
        /// </summary>
        /// <param name="message">The received message/reason</param>
        public DisconnectUserPacket(string message)
        {
            Type(PacketNumber);
            WriteInt(60);
            WriteString(message);
        }
    }
}
