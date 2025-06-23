using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.MapServer
{
    public class MapSwapPacket : PacketWriter
    {
        private const int PacketNumber = 1709;

        /// <summary>
        /// Sends the tamer to the target map and position.
        /// </summary>
        /// <param name="tamer">The tamer to spawn.</param>
        public MapSwapPacket(string serverAddress, string serverPort, int mapId, int x, int y)
        {
            Type(PacketNumber);
            WriteString(serverAddress);
            WriteInt(Convert.ToInt32(serverPort));
            WriteInt(mapId);
            WriteInt(x);
            WriteInt(y);
            WriteString(Convert.ToString(mapId));
        }
       
    }

    public class LocalMapSwapPacket : PacketWriter
    {
        private const int PacketNumber = 1711;
        public LocalMapSwapPacket(int tGeneralHandler,int dGeneralHandler, int tx, int ty,int dx,int dy)
        {
            Type(PacketNumber);
            WriteInt(tGeneralHandler);
            WriteInt(dGeneralHandler);
            WriteInt(tx);
            WriteInt(ty);
            WriteInt(dx);
            WriteInt(dy);
        }
    }
}