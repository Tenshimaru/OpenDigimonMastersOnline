using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DigimonWalkPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Default digimon movimentation packet.
        /// </summary>
        /// <param name="digimon">The digimon that is moving</param>
        public DigimonWalkPacket(DigimonModel digimon)
        {
            Type(PacketNumber);
            WriteByte(6);
            WriteShort(1);
            WriteUInt(digimon.GeneralHandler);
            WriteInt(digimon.Location.X);
            WriteInt(digimon.Location.Y);
            WriteInt(0);
        }
        public DigimonWalkPacket(int X, int Y,uint Handler)
        {
            Type(PacketNumber);
            WriteByte(6);
            WriteShort(1);
            WriteUInt(Handler);
            WriteInt(X);
            WriteInt(Y);
            WriteInt(0);
        }
    }
}