using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DigimonSkillCapUpResultPacket : PacketWriter
    {
        private const int PacketNumber = 3245;

        public DigimonSkillCapUpResultPacket(Enums.ClientEnums.DigimonSkillCapIncreaseResultEnum result, uint formSlot, DigimonEvolutionModel dEvo, uint invSlot, uint itemId)
        {
            Type(PacketNumber);
            WriteInt(result.GetHashCode());         // The attempt Result
            WriteUInt(formSlot);                    // The evolution slot
            WriteBytes(dEvo.ToArray());             // The evolution itself
            WriteUInt(invSlot);                     // Slot where the item was
            WriteUInt(itemId);                      // The item Type(item id? L_Type? S_Type?)
        }
    }
}