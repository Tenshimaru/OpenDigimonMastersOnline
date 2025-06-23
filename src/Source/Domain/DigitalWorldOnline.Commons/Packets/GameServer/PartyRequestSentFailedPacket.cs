using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyRequestSentFailedPacket : PacketWriter
    {
        private const int PacketNumber = 2302;

        /// <summary>
        /// Unable to send party request.
        /// </summary>
        /// <param name="result">Fail result enumeration</param>
        /// <param name="targetName">Target tamer name</param>
        public PartyRequestSentFailedPacket(PartyRequestFailedResultEnum result, string targetName)
        {
            Type(PacketNumber);
            WriteInt(result.GetHashCode());
            WriteString(targetName);
        }
    }
}