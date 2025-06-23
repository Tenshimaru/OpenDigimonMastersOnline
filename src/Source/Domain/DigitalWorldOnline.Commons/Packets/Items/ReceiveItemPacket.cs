using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class ReceiveItemPacket : PacketWriter
    {
        private const int PacketNumber = 3936;

        /// <summary>
        /// Receives and highlight a new item to the target inventory.
        /// </summary>
        /// <param name="item">The item received.</param>
        /// <param name="inventoryType">The target inventory enumeration.</param>
        /// <param name="slot">The affected slot.</param>
        public ReceiveItemPacket(ItemModel item, InventoryTypeEnum inventoryType, int slot = 0)
        {
            Type(PacketNumber);
            WriteByte((byte)inventoryType);
            WriteByte((byte)item.Slot);
            WriteInt(item.ItemId);
            WriteShort((short)item.Amount);
            WriteByte(0);

            if (item.RemainingMinutes() == 4294967280)
            {
                WriteUInt(item.RemainingMinutes());
            }
            else
            {
                var ts = UtilitiesFunctions.RemainingTimeMinutes((int)item.RemainingMinutes());

                WriteInt(ts);
            }
            WriteByte(0);
        }
    }
}