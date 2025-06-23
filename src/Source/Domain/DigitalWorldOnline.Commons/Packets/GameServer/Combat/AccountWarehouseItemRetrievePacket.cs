using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class AccountWarehouseItemRetrievePacket : PacketWriter
    {
        private const int PacketNumber = 3931;

        /// </summary>
        /// <param name="giftStorage">The list of Gift Storage</param>
        public AccountWarehouseItemRetrievePacket(ItemModel item,int wareSlot)
        {
            Type(PacketNumber);
            WriteInt(item.Slot);
            WriteInt(0);
            WriteInt(wareSlot);
            WriteInt(item.ItemId);
            WriteInt(item.Amount);
            WriteByte(0);
            if (item.RemainingMinutes() == 4294967280)
            {
                WriteUInt(item.RemainingMinutes());
            }
            else
            {
                WriteInt(UtilitiesFunctions.RemainingTimeMinutes((int)item.RemainingMinutes()));
            }
            WriteInt(0);

        }
    }
}