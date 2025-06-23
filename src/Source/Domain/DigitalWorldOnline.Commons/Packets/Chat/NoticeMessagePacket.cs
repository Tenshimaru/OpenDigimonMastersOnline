using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Chat
{
    public class NoticeMessagePacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Sends the message to the general chat with SYSTEM prefix.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public NoticeMessagePacket(string message)
        {
            Type(PacketNumber);
            WriteByte(10);
            WriteByte(1);
            WriteString($"{message}");
            WriteByte(0);

        }
    }
}

