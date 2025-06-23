using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.PersonalShop
{
    public class PersonalShopSellItemPacket : PacketWriter
    {
        private const int PacketNumber = 1514;

        /// <summary>
        /// Pop up the personal shop window.
        /// </summary>
        /// <param name="slot">The item id used to open the shop</param>
        /// <param name="count">The item id used to open the shop</param>
        public PersonalShopSellItemPacket(int slot, int count)
        {
            Type(PacketNumber);
            WriteInt(slot);
            WriteInt(count);
        }
    }
}