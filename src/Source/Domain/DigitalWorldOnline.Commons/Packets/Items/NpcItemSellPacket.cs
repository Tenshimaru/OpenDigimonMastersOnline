using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class NpcItemSellPacket : PacketWriter
    {
        private const int PacketNumber = 3916;

        /// <summary>
        /// Unexpected fail upon NPC item purchasing.
        /// </summary>
        /// <param name="currentBits">Current bits amount</param>
        
        public NpcItemSellPacket(long currentBits, ItemModel soldItem)
        {
            Type(PacketNumber);
            WriteInt64(currentBits);
            WriteByte(1);
            WriteByte((byte)soldItem.Slot);
            WriteInt(soldItem.ItemId);
            WriteShort((short)soldItem.Amount);
            //WriteByte(0); //nRate
            //WriteByte(0); //Remaining time
        }
        
        public NpcItemSellPacket(long currentBits)
        {
            Type(PacketNumber);
            WriteInt64(currentBits);
            WriteByte(0);
            WriteByte(0);
            WriteInt(0);
            WriteShort(0);
            //WriteByte(0); //nRate
            //WriteByte(0); //Remaining time
        }
    }
}