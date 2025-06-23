using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class SyncConditionPacket : PacketWriter
    {
        private const int PacketNumber = 1070;

		/// <summary>
		/// Synchronizes the state condition of the target.
		/// </summary>
		/// <param name="handler">The handler of the target.</param>
        /// <param name="condition">The target condition to update.</param>
		public SyncConditionPacket(int handler, ConditionEnum condition)
        {
            Type(PacketNumber);
            WriteInt(handler);
            WriteInt(condition.GetHashCode());
        }

        /// <summary>
		/// Synchronizes the state condition of the target for the tamer shop.
		/// </summary>
		/// <param name="handler">The handler of the target.</param>
        /// <param name="condition">The target condition to update.</param>
		public SyncConditionPacket(int handler, ConditionEnum condition, string shopName)
        {
            Type(PacketNumber);
            WriteInt(handler);
            WriteInt(condition.GetHashCode());
            WriteString(shopName);
        }
    }
}