using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TamerRelationsPacket : PacketWriter
    {
        private const int PacketNumber = 2404;

        /// <summary>
        /// Loads the tamer's relationships (friends and foes).
        /// </summary>
        /// <param name="friends">The tamer's friends.</param>
        /// <param name="foes">The tamer's foes.</param>
        public TamerRelationsPacket(IList<CharacterFriendModel> friends, IList<CharacterFoeModel> foes)
        {
            Type(PacketNumber);
            WriteShort((short)friends.Count);
            foreach (var friend in friends)
            {
                WriteByte(Convert.ToByte(friend.Connected));
                WriteString(friend.Friend?.Name ?? "Friend");

                if(string.IsNullOrEmpty(friend.Annotation))
                    WriteByte(0);
                else
                    WriteString(friend.Annotation);
            }

            WriteShort((short)foes.Count);
            foreach (var foe in foes)
            {
                WriteString(foe.Name);
                if (string.IsNullOrEmpty(foe.Annotation))
                    WriteByte(0);
                else
                    WriteString(foe.Annotation);
            }
        }
    }
}
