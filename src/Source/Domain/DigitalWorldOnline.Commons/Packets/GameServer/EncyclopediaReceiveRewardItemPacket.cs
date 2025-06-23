using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class EncyclopediaReceiveRewardItemPacket : PacketWriter
    {
        private const int PacketNumber = 3235;

        public EncyclopediaReceiveRewardItemPacket(ItemModel item, int digimonId)
        {
            Type(PacketNumber);

            WriteUInt((uint)item.ItemId);
            WriteUShort((ushort)item.Amount);
        }
    }
}