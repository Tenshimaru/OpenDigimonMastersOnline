using DigitalWorldOnline.Commons.DTOs.Assets;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class GotchaStartPacket : PacketWriter
    {
        private const int PacketNumber = 3956;

        /// <summary>
        /// Open Rare Machine
        /// </summary>
        public GotchaStartPacket(GotchaAssetModel Gotcha)
        {
            Type(PacketNumber);
            WriteInt(Gotcha.RareItems.Count);

            foreach (var rareItem in Gotcha.RareItems)
            {
                WriteUInt((uint)rareItem.RareItem);
                WriteUInt((uint)rareItem.RareItemCnt);
            }

            int totalQuanty = Gotcha.Items.Sum(item => item.Quanty);
            WriteInt(totalQuanty);
            WriteInt(100);
        }
    }
}