using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.PersonalShop
{
    public class PersonalShopPacket : PacketWriter
    {
        private const int PacketNumber = 1510;

        /// <summary>
        /// Pop up the personal shop window.
        /// </summary>
        /// <param name="shopAction">The target action enum to choose the shop type</param>
        /// <param name="itemId">The item id used to open the shop</param>
        public PersonalShopPacket(TamerShopActionEnum shopAction, int itemId)
        {
            Type(PacketNumber);
            WriteInt(shopAction.GetHashCode());
            WriteInt(itemId);
        }

        /// <summary>
        /// Updates the tamer shop store window as open.
        /// </summary>
        /// <param name="itemId">The item id used to open the shop</param>
        public PersonalShopPacket(int itemId)
        {
            Type(1511);
            WriteInt(100);
            WriteInt(itemId);
        }

        public PersonalShopPacket()
        {
            Type(1512);
            WriteInt(100);
        }
    }
}