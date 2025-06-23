using DigitalWorldOnline.Commons.Enums.Party;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartyChangeLootTypePacket : PacketWriter
    {
        private const int PacketNumber = 2309;

       
        public PartyChangeLootTypePacket(PartyLootShareTypeEnum lootType, PartyLootShareRarityEnum rareType)
        {
            Type(PacketNumber);
            WriteInt((int)lootType);
            WriteByte((byte)rareType);
            WriteByte(1);          
        }
    }
}
