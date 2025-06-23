using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildNoticeUpdatePacket : PacketWriter
    {
        private const int PacketNumber = 2126;

        /// <summary>
        /// Updates the current guild notice message.
        /// </summary>
        /// <param name="newMessage">The new message</param>
        public GuildNoticeUpdatePacket(string newMessage)
        {
            Type(PacketNumber);
            WriteString(newMessage);
        }
    }
}