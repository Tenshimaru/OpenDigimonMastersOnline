using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class UpdateItemDurationPacket : PacketWriter
    {
        private const int PacketNumber = 3933;

        /// <summary>
        /// Updates the items collection as expired.
        /// </summary>
        /// <param name="expiredItemList">The expired items collection</param>
        public UpdateItemDurationPacket(List<ItemListModel> expiredItemList)
        {
            Type(PacketNumber);
            WriteByte(0);
            WriteByte(12);
            WriteInt(102);
            WriteInt(2);

            foreach (var itemList in expiredItemList)
            {
                foreach (var expiredItem in itemList.Items)
                {
                    Type(PacketNumber);
                    WriteByte((byte)itemList.Type);
                    WriteByte(2);
                    WriteInt(expiredItem.ItemId);
                    WriteInt(1);
                }
            }
        }
    }
}