using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ItemScanFailPacket : PacketWriter
    {
        private const int PacketNumber = 3987;

        /// <summary>
        /// Returns the fail result of the scan operation.
        /// </summary>
        /// <param name="currentBits">Current tamer bits</param>
        /// <param name="itemSlot">Source slot of the scanned item</param>
        /// <param name="itemId">Scanned item id</param>
        public ItemScanFailPacket(long currentBits, int itemSlot, int itemId)
        {
            Type(PacketNumber);
            WriteInt(0);
            WriteInt64(0);
            WriteInt64(currentBits);
            WriteInt(itemSlot);
            WriteInt(itemId);
            WriteShort(1);
            WriteInt(1);

            //Receives the scan item as reward
            WriteInt(itemId);
            WriteInt(1);
            WriteBytes(new byte[52]);
            WriteInt(0xFFFF);
            WriteInt(0);
        }
    }
}