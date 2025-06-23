using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class SealFavoritePacket : PacketWriter
    {
        private const int PacketNumber = 1334;

        /// <summary>
        /// Add/remove the target seal as favorite
        /// </summary>
        /// <param name="sealId">Target seal id</param>
        /// <param name="favorited">Set or remove</param>
        public SealFavoritePacket(short sealId, byte favorited)
        {
            Type(PacketNumber);
            WriteShort(sealId);
            WriteByte(favorited);
        }
    }
}