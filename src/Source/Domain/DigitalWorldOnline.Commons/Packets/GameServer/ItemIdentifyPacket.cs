using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ItemIdentifyPacket : PacketWriter
    {
        private const int PacketNumber = 3968;

        /// <summary>
        /// Identified item success.
        /// </summary>
        /// <param name="slot">Identified item slot</param>
        /// <param name="identifiedItem">Identified item</param>
        public ItemIdentifyPacket(short slot, ItemModel identifiedItem)
        {
            Type(PacketNumber);
            WriteShort(slot);
            WriteByte(identifiedItem.Power);
            WriteByte(identifiedItem.RerollLeft);
            foreach (var status in identifiedItem.AccessoryStatus.OrderBy(x => x.Slot))
            {
                WriteShort((short)status.Type);
            }

            foreach (var status in identifiedItem.AccessoryStatus.OrderBy(x => x.Slot))
            {
                WriteShort(status.Value);
            }
        }
    }
}