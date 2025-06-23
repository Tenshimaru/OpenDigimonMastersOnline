using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GlobalMessagePacket: PacketWriter
    {
        private const int PacketNumber = 16006;

    
        /// <param name="targetHandler">Target handler</param>
        public GlobalMessagePacket(int targetHandler, string attackerName, int attackerType, int itemId)
        {
            Type(PacketNumber);
            WriteShort(1605);
            WriteInt(targetHandler);
            WriteString(attackerName);
            WriteInt(attackerType);
            WriteInt(itemId);
        }
      
    }
}