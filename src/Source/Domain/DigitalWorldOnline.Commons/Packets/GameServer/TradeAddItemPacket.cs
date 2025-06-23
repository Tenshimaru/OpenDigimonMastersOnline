using DigitalWorldOnline.Commons.Writers;
using System.Net.Sockets;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TradeAddItemPacket : PacketWriter
    {
        private const int PacketNumber = 1508;

        /// <summary>
        /// Party request sent.
        /// </summary>
        /// <param name="TargetHandle">Target tamer handle accept</param>
        /// <param name="item">Current trade item in array</param>
        /// <param name="tradeSlot"> trade slot</param>
        /// <param name="inventorySlot"> Inventory slot</param>
        public TradeAddItemPacket(int targetHandle, byte[] item, byte tradeSlot, int inventorySlot)
        {
            Type(PacketNumber);
            WriteInt(targetHandle);
            WriteBytes(item);
            WriteByte(tradeSlot);
            WriteInt(inventorySlot);
        }
    }
}