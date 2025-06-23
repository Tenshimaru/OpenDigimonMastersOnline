using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class LoadRewardStoragePacket : PacketWriter
    {
        private const int PacketNumber = 16001;

        /// </summary>
        /// <param name="giftStorage">The list of Gift Storage</param>
        public LoadRewardStoragePacket(ItemListModel giftStorage)
        {
            Type(PacketNumber);
            WriteShort(giftStorage.Count);
            WriteBytes(giftStorage.GiftToArray());
        }
    }
}