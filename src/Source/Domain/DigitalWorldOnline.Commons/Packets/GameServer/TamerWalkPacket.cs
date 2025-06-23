using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class TamerWalkPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Default tamer movimentation packet.
        /// </summary>
        /// <param name="tamer">The tamer that is moving</param>
        public TamerWalkPacket(CharacterModel tamer)
        {
            Type(PacketNumber);
            WriteByte(5);
            WriteShort(1);
            WriteUInt(tamer.GeneralHandler);
            WriteInt(tamer.Location.X);
            WriteInt(tamer.Location.Y);
            WriteInt(0);
        }
        public TamerWalkPacket(int X,int Y, uint Handle)
        {
            Type(PacketNumber);
            WriteByte(5);
            WriteShort(1);
            WriteUInt(Handle);
            WriteInt(X);
            WriteInt(Y);
            WriteInt(0);
        }
    }
}