using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class DigimonTranscendenceSuccessPacket : PacketWriter
    {
        private const int PacketNumber = 16040;

        /*public DigimonTranscendenceSuccessPacket(int Result, byte targetSlot, DigimonHatchGradeEnum scale, int tamerMoney)
        {
            Type(PacketNumber);
            WriteInt(Result);               // result: 0 - succes, 1 - fail
            WriteByte(targetSlot);          // DigimonTranscendencePos: 0, 1, 2, 3
            WriteByte((byte)scale);         // HatchLevel
            WriteInt((int)50000);           // DigimonTranscendenceMoney
            WriteInt(0);                    // Money
            WriteInt(tamerMoney);           // Exp
            WriteInt(0);
            WriteInt64(0);
        }*/

        public DigimonTranscendenceSuccessPacket(int Result, byte targetSlot, DigimonHatchGradeEnum scale, long price, int tamerMoney, int exp)
        {
            Type(PacketNumber);
            WriteInt(Result);               // result: 0 - succes, 1 - fail
            WriteByte(targetSlot);          // DigimonTranscendencePos: 0, 1, 2, 3
            WriteByte((byte)scale);         // HatchLevel
            WriteInt((int)price);           // DigimonTranscendenceMoney
            WriteInt(tamerMoney);           // Money
            WriteInt(exp);                  // Exp
        }

    }
}