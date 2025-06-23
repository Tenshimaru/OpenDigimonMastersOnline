using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Chat
{
    public class PartyMessagePacket : PacketWriter
    {
        private const int PacketNumber = 2304;

        /// <summary>
        /// Sends a message to the entire party.
        /// </summary>
        /// <param name="senderName">The name of the party member who's sent the message.</param>
        /// <param name="message">The message itself.</param>
        public PartyMessagePacket(string senderName, string message)
        {
            Type(PacketNumber);
            WriteString(senderName);
            WriteString(message);
        }
    }
}