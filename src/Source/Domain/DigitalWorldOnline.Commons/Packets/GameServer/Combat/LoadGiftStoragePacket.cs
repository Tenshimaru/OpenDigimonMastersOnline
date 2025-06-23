using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer.Combat
{
    public class LoadGiftStoragePacket : PacketWriter
    {
        private const int PacketNumber = 3935;
   
        /// </summary>
        /// <param name="giftStorage">The list of Gift Storage</param>
        public LoadGiftStoragePacket(ItemListModel giftStorage)
        {
            Type(PacketNumber);
            WriteShort(giftStorage.Count);
            WriteBytes(giftStorage.NewGiftToArray());
        }
    }
}