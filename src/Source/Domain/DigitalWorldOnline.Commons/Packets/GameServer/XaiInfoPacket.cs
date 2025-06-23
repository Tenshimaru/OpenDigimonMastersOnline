using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class XaiInfoPacket : PacketWriter
    {
        private const int PacketNumber = 16033;

        /// <summary>
        /// Show the XAI max resources.
        /// </summary>
        /// <param name="xai">The target XAI</param>
        public XaiInfoPacket(CharacterXaiModel? xai)
        {
            Type(PacketNumber);
            WriteInt(xai?.XGauge ?? 0);
            WriteShort(xai?.XCrystals ?? 0);
        }


        //TODO: temp
        public XaiInfoPacket()
        {
            Type(PacketNumber);
            WriteInt(0);
            WriteShort(0);
        }
    }
}
