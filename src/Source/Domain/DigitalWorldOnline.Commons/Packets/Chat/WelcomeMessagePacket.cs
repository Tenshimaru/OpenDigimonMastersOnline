using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Chat
{
    public class WelcomeMessagePacket : PacketWriter
    {
        private const int PacketNumber = 1047;

        /// <summary>
        /// Sends an welcome message to the general chat.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public WelcomeMessagePacket(string message)
        {
            Type(PacketNumber);
            WriteByte(1);
            WriteString(message);
        }
    }
}