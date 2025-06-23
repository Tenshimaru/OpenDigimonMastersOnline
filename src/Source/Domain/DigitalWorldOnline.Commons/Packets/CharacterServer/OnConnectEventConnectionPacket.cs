using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.CharacterServer
{
    public class OnConnectEventConnectionPacket : PacketWriter
    {
        private const int PacketNumber = 65535;

        /// <summary>
        /// Used on the listener for OnConnectEvent
        /// </summary>
        /// <param name="handshake">The client-server handshake</param>
        public OnConnectEventConnectionPacket(short handshake)
        {
            Type(PacketNumber);
            WriteShort(handshake);
        }
    }
}
