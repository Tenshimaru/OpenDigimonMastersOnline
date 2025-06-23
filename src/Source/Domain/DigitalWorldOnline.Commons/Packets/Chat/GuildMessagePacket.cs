using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Chat
{
    public class GuildMessagePacket : PacketWriter
    {
        private const int PacketNumber = 2114;

        /// <summary>
        /// Sends the message to the entire guild.
        /// </summary>
        /// <param name="senderName">The name of the guild member who's sent the message.</param>
        /// <param name="message">The sent message.</param>
        public GuildMessagePacket(string senderName, string message)
        {
            Type(PacketNumber);
            WriteString(senderName);
            WriteString(message);
        }
    }
}