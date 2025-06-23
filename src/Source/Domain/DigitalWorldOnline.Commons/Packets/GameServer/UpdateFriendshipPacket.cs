using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class UpdateFriendshipPacket : PacketWriter
    {
        private const int PacketNumber = 1024;

        /// <summary>
        /// Updates the partner current friendship.
        /// </summary>
        /// <param name="character">The tamer to be updated</param>
        public UpdateFriendshipPacket(short friendship)
        {
            Type(PacketNumber);
            WriteShort(friendship);
        }
    }
}