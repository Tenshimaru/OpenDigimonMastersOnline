using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class EvolutionArmorUnlockedPacket : PacketWriter
    {
        private const int PacketNumber = 3238;

        public EvolutionArmorUnlockedPacket(short result,byte success)
        {
            Type(PacketNumber);
            WriteShort(result);
            WriteByte(success);
        }
    }
}