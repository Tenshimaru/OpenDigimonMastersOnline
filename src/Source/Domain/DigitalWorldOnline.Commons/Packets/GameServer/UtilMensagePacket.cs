using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class UtilMensagePacket : PacketWriter
    {
        private const int PacketNumber = 4125;

        public UtilMensagePacket(ItemStoneEnum type)
        {
           Type(PacketNumber);
           WriteInt((int)type);
        }
    }
}
