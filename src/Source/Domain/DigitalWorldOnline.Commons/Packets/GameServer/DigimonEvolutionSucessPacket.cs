using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DigimonEvolutionSucessPacket : PacketWriter
    {
        private const int PacketNumber = 1028;

        /// <summary>
        /// Evolves the current digimon to a new form.
        /// </summary>
        /// <param name="tamerHandler">Digimon owner handler.</param>
        /// <param name="digimonHandler">Target digimon handler</param>
        /// <param name="newType">The new form type</param>
        /// <param name="evoEffect">Evolution effect enumeration</param>
        public DigimonEvolutionSucessPacket(uint tamerHandler, uint digimonHandler, int newType, DigimonEvolutionEffectEnum evoEffect)
        {
            Type(PacketNumber);
            WriteUInt(digimonHandler);
            WriteUInt(tamerHandler);
            WriteInt(newType);
            WriteByte((byte)evoEffect);
            WriteShort(254);
        }
    }
}