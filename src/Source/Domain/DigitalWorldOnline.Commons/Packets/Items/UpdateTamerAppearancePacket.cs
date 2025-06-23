using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class UpdateTamerAppearancePacket : PacketWriter
    {
        private const int PacketNumber = 1310;

        /// <summary>
        /// Updates the tamer current appearance.
        /// </summary>
        /// <param name="appearanceHandle">Tamer appearance handler value.</param>
        /// <param name="slot">Affected equipment slot.</param>
        /// <param name="item">Updated item.</param>
        public UpdateTamerAppearancePacket(int appearanceHandle, byte slot, ItemModel item, byte enable)
        {
            Type(PacketNumber);
            WriteInt(appearanceHandle);
            WriteByte(slot);
            WriteInt(item.ItemId);
            if (item.ItemInfo != null)
            {

                if (item.RemainingMinutes() == 4294967280)
                {
                    WriteUInt(item.RemainingMinutes());
                }
                else
                {
                    WriteInt(UtilitiesFunctions.RemainingTimeMinutes((int)item.RemainingMinutes()));
                }
            }
            else
            {
                WriteInt(0);
            }
            WriteInt(0);
            WriteByte(enable);
        }
    }
}