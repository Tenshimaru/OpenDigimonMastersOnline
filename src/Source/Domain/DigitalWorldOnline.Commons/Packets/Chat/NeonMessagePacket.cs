using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Chat
{
    public class NeonMessagePacket : PacketWriter
    {
        private const int PacketNumber = 16034;

        /// <summary>
        /// Sends the neon message to the target chat.
        /// </summary>
        /// <param name="neonType">The target neon enumeration</param>
        /// <param name="tamerName">Receiver tamer name</param>
        /// <param name="sourceIdentifier">The neon message origin identifier</param>
        /// <param name="resultIdentifier">The neon message result identifier</param>
        public NeonMessagePacket(NeonMessageTypeEnum neonType, string tamerName, int sourceIdentifier, int resultIdentifier)
        {
            Type(PacketNumber);
            WriteShort((short)neonType);
            WriteString(tamerName);
            WriteInt(sourceIdentifier);
            WriteInt(resultIdentifier);
        }
    }
}