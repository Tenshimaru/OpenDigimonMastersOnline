using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyRequestSentSuccessPacket : PacketWriter
    {
        private const int PacketNumber = 2301;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="senderName">Sender tamer name</param>
        public PartyRequestSentSuccessPacket(string senderName)
        {
            Type(PacketNumber);
            WriteString(senderName);
        }
    }
}