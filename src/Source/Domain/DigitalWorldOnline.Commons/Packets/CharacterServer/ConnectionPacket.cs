using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.CharacterServer
{
    public class ConnectionPacket : PacketWriter
    {
        private const int PacketNumber = -2;

        /// <summary>
        /// Used on client requesting connection with the server.
        /// </summary>
        /// <param name="handshake">The server-client handshake</param>
        /// <param name="handshakeTimestamp">Timestamp when the handshake has been accepted</param>
        public ConnectionPacket(short handshake, uint handshakeTimestamp)
        {
            Type(PacketNumber);
            WriteShort(handshake);
            WriteUInt(handshakeTimestamp);
        }
    }
}
