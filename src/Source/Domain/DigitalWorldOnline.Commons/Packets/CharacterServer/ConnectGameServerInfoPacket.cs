using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.CharacterServer
{
    public class ConnectGameServerInfoPacket : PacketWriter
    {
        private const int PacketNumber = 1308;

        /// <summary>
        /// Send game server connection information.
        /// </summary>
        public ConnectGameServerInfoPacket(string ipAddress, string port, short mapId)
        {
            Type(PacketNumber);

            WriteString(ipAddress);
            WriteInt(int.Parse(port));
            WriteInt(mapId);
        }
    }
}
