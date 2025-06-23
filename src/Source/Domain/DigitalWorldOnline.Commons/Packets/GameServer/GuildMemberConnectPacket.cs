using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GuildMemberConnectPacket : PacketWriter
    {
        private const int PacketNumber = 2111;

        /// <summary>
        /// Send a notification upon guild member connect to the game.
        /// </summary>
        /// <param name="character">Member information</param>
        public GuildMemberConnectPacket(CharacterModel character)
        {
            Type(PacketNumber);
            WriteString(character.Name);
            WriteByte(character.Level);
            WriteShort(character.Location.MapId);
            WriteByte(character.Channel);
        }
    }
}