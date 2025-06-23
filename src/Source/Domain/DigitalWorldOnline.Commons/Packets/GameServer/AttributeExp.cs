using DigitalWorldOnline.Commons.Writers;
using DigitalWorldOnline.Commons.Utils;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class NatureExpPacket :PacketWriter
    {
        private const int PacketNumber = 1602; // Unique packet identifier for Nature Exp should i change that to 1603? yes but at client its 1602??

        /// <summary>
        /// Constructs the packet to send nature experience for a Digimon.
        /// </summary>
        /// <param name="mainType">Main attribute type (0-1; 255 if failure).</param>
        /// <param name="subType">Sub attribute type (e.g., specific nature types, 255 if failure).</param>
        /// <param name="exp">Experience value (positive or negative).</param>
        public NatureExpPacket(byte mainType,byte subType,short exp)
        {
            // Packet Type
            Type(PacketNumber);

            // Write main type (0-1 or 255 for failure)
            WriteByte(mainType);

            // Write sub type (e.g., nature-specific attribute; 255 if failure)
            WriteByte(subType);

            // Write experience change (nExp)
            WriteShort(exp);
        }
    }
}