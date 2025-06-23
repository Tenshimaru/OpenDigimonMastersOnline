using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class ItemSocketIdentifyPacket : PacketWriter
    {
        private const int PacketNumber = 3929;

        public ItemSocketIdentifyPacket(ItemModel item, int Money)
        {
            Type(PacketNumber);
            WriteByte(item.Power);
            WriteInt(Money);
            WriteInt(0);

        }
    }
}
