using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.PersonalShop
{
    public class ConsignedShopBoughtItemPacket : PacketWriter
    {
        private const int PacketNumber = 1518;

        /// <summary>
        /// Pop up the personal shop window.
        /// </summary>
        /// <param name="shopAction">The target action enum to choose the shop type</param>
        /// <param name="itemId">The item id used to open the shop</param>
        public ConsignedShopBoughtItemPacket(TamerShopActionEnum shopAction, int slot, int count)
        {
            Type(PacketNumber);
            WriteInt(shopAction.GetHashCode());
            WriteInt(slot);
            WriteInt(count);
        }

        /// <summary>
        /// Pop up the personal shop window.
        /// </summary>
        /// <param name="shopAction">The target action enum to choose the shop type</param>
        /// <param name="itemId">The item id used to open the shop</param>
        public ConsignedShopBoughtItemPacket(bool isFailed = true)
        {
            Type(PacketNumber);
            WriteInt(1);
        }
    }
}