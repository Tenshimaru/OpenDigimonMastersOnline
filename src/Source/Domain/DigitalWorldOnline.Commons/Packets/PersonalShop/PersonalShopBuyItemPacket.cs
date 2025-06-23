using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.PersonalShop
{
    public class PersonalShopBuyItemPacket : PacketWriter
    {
        private const int PacketNumber = 1513;

        /// <summary>
        /// Pop up the personal shop window.
        /// </summary>
        /// <param name="shopAction">The target action enum to choose the shop type</param>
        /// <param name="slot">The item id used to open the shop</param>
        /// <param name="count">The item id used to open the shop</param>
        public PersonalShopBuyItemPacket(TamerShopActionEnum shopAction, int slot, int count)
        {
            Type(PacketNumber);
            WriteInt(shopAction.GetHashCode());
            WriteInt(slot);
            WriteInt(count);
        }
        
        /// <summary>
        /// Pop up the personal shop window.
        /// </summary>
        public PersonalShopBuyItemPacket(TamerShopActionEnum shopAction)
        {
            Type(PacketNumber);
            WriteInt(shopAction.GetHashCode());
        }
    }
}