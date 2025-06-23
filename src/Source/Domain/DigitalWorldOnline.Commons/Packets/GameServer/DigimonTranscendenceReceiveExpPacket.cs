using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DigimonTranscendenceReceiveExpPacket : PacketWriter
    {
        private const int PacketNumber = 16039;

        public DigimonTranscendenceReceiveExpPacket(AcademyInputType inputType,byte targetSlot,short digimonCount,List<short> targetDeleteSlots,short itemSlot,ItemModel targetItem,short successRate,long chargeExp,long targetPartnerFinalExp)
        {
            Type(PacketNumber);
            WriteInt(0); // Sucess
            WriteByte((byte)inputType);
            WriteByte((byte)targetSlot);
            WriteShort(digimonCount);

            foreach (var targetToDeleteSlot in targetDeleteSlots)
            {
                WriteShort(targetToDeleteSlot);
            }

            WriteShort(1);
            WriteShort(itemSlot);
            WriteBytes(targetItem.ToArray());
            WriteShort((short)successRate);
            WriteInt64(chargeExp);
            WriteShort(0);
            WriteInt64(targetPartnerFinalExp);
        }
    }
}