using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class LoadInventoryPacket : PacketWriter
    {
        private const int PacketNumber = 16038;

        /// <summary>
        /// Load the itens on tamer's target inventory.
        /// </summary>
        /// <param name="inventory">The itens to load</param>
        /// <param name="inventoryType">Inventory type</param>
        public LoadInventoryPacket(ItemListModel inventory, InventoryTypeEnum inventoryType) //TODO: utilizar o enum no proprio itemlist
        {
            Type(PacketNumber);
            WriteInt(0);
            WriteInt64(inventory.Bits);
            WriteByte((byte)inventoryType);
            WriteShort(inventory.Size);
            WriteBytes(inventory.ToArray());
        }
    }
}
