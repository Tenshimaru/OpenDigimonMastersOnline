using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.MapServer
{
    public class UnloadDropsPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Unload the target drop.
        /// </summary>
        /// <param name="drop">The drop to be unloaded.</param>
        public UnloadDropsPacket(Drop drop)
        {
            Type(PacketNumber);
            WriteByte(4);
            WriteShort(1);
            WriteInt(drop.GeneralHandler);
            WriteInt(drop.Location.X);
            WriteInt(drop.Location.Y);
            WriteInt(0);
        }

        public UnloadDropsPacket(int dropHandler)
        {
            Type(PacketNumber);
            WriteByte(4);
            WriteShort(1);
            WriteInt(dropHandler);
            WriteInt(0);
            WriteInt(0);
            WriteInt(0);
        }
    }
}