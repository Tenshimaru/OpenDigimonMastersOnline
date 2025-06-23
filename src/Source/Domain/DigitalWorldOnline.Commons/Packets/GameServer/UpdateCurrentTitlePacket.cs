using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class UpdateCurrentTitlePacket : PacketWriter
    {
        private const int PacketNumber = 15;

        /// <summary>
        /// Updates the current tamer title.
        /// </summary>
        /// <param name="handler">Target tamer handler</param>
        /// <param name="titleId">Target title id</param>
        public UpdateCurrentTitlePacket(int handler, short titleId)
        {
            Type(PacketNumber);
            WriteUInt((uint)handler);
            WriteShort(titleId);
        }
    }
}