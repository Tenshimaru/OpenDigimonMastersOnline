using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.MapServer
{
    public class UnloadTamerPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Despawns the target tamer.
        /// </summary>
        /// <param name="tamer">The tamer to despawn.</param>
        public UnloadTamerPacket(CharacterModel character)
        {
            Type(PacketNumber);
            WriteByte(4);
            WriteShort(2);
            WriteInt(character.GeneralHandler);
            WriteInt(character.Location.X);
            WriteInt(character.Location.Y);

            WriteInt(character.Partner.GeneralHandler);
            WriteInt(character.Partner.Location.X);
            WriteInt(character.Partner.Location.Y);

            WriteInt(0);
        }
    }
}