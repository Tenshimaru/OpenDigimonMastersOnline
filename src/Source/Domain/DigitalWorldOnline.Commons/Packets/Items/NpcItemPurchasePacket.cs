using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class NpcItemPurchasePacket : PacketWriter
    {
        private const int PacketNumber = 3915;

        /// <summary>
        /// Unexpected fail upon NPC item purchasing.
        /// </summary>
        /// <param name="currentBits">Current bits amount</param>
        public NpcItemPurchasePacket(long currentBits)
        {
            Type(PacketNumber);
            WriteInt64(currentBits);
            WriteByte(0);
            WriteByte(0);
        }
        
        public NpcItemPurchasePacket(long currentBits, ItemModel purchasedItem)
        {
            Type(PacketNumber);
            WriteInt64(currentBits);
            WriteByte(1);
            WriteByte((byte)purchasedItem.Slot);
            WriteInt(purchasedItem.ItemId);
            WriteShort((short)purchasedItem.Amount);
            WriteByte(0); //nRate
            WriteByte(0); //Remaining time
        }
    }
}