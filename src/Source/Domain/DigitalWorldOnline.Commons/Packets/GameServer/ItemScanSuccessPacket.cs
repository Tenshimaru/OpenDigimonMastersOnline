using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ItemScanSuccessPacket : PacketWriter
    {
        private const int PacketNumber = 3987;

        /// <summary>
        /// Returns the success message on item scanning.
        /// </summary>
        /// <param name="bitsCost">Total scan cost</param>
        /// <param name="remainingBits">Bits remaining on tamer inventory</param>
        /// <param name="slotToScan">Tamer inventory slot position to scan</param>
        /// <param name="scannedItemId">Scanned item id</param>
        /// <param name="scannedAmount">Total scanned item amount</param>
        /// <param name="rewards">Reward list (slot and item)</param>
        public ItemScanSuccessPacket(long bitsCost, long remainingBits, int slotToScan, int scannedItemId, short scannedAmount, Dictionary<int, ItemModel> rewards)
        {
            Type(PacketNumber);
            WriteInt(0);
            WriteInt64(bitsCost * -1);
            WriteInt64(remainingBits);
            WriteInt(slotToScan);
            WriteInt(scannedItemId);
            WriteShort(scannedAmount);

            WriteInt(rewards.Count);
            foreach (var reward in rewards)
            {
                WriteInt(reward.Key);
                WriteBytes(reward.Value.ToArray());
            }
        }
    }
}