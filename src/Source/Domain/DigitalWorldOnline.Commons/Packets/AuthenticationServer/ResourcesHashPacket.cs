using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.AuthenticationServer
{
    public class ResourcesHashPacket : PacketWriter
    {
        private const int PacketNumber = 10003;

        /// <summary>
        /// Sends the resource hash string to the client.
        /// </summary>
        /// <param name="hash">Resources hash string</param>
        public ResourcesHashPacket(string hash)
        {
            Type(PacketNumber);
            WriteShort((short)(hash.Length / 2));
            WriteBytes(ToByteArray(hash));
        }

        private static byte[] ToByteArray(string hexString)
        {
            int NumberChars = hexString.Length;
            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);

            return bytes;
        }
    }
}
