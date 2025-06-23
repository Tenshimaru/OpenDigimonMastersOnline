using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.CharacterServer
{
    public class ConnectGameServerPacket : PacketWriter
    {
        private const int PacketNumber = 1703;

        /// <summary>
        /// Connecting to the game server.
        /// </summary>
        public ConnectGameServerPacket()
        {
            Type(PacketNumber);
        }
    }
}
