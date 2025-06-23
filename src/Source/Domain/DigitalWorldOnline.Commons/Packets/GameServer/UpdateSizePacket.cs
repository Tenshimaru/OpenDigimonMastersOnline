using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class UpdateSizePacket : PacketWriter
    {
        private const int PacketNumber = 9942;

        /// <summary>
        /// Updates the current partner size.
        /// </summary>
        public UpdateSizePacket(ushort handler, short size)
        {
            Type(PacketNumber);
            WriteUInt(handler);
            WriteInt(size);
            WriteShort(0); //ChangeType
        }
        public UpdateSizePacket(string OfferName, int MapID)
        {
            Type(1114);
            WriteByte(3); //Case
            WriteByte(1); //nValue
            WriteInt(MapID);
            WriteString(OfferName); //ChangeType
        }

        public enum ChangeType
        {
            /// <summary>
            /// A permanent size change.
            /// </summary>
            Permanent = 0,

            /// <summary>
            /// A temporary size change. Lasts 3 min
            /// </summary>
            Temporary = 2
        }
    }
}