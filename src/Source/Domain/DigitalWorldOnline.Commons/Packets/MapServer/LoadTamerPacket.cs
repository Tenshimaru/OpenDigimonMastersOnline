using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.MapServer
{
    public class LoadTamerPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

        /// <summary>
        /// Spawns the target tamer.
        /// </summary>
        /// <param name="tamer">The tamer to spawn.</param>
        public LoadTamerPacket(CharacterModel tamer)
        {
            Type(PacketNumber);
            WriteByte(3);
            WriteShort(2);

            WriteInt(tamer.Location.X);
            WriteInt(tamer.Location.Y);
            WriteUInt(tamer.GeneralHandler);
            WriteInt(tamer.Model.GetHashCode());
            WriteInt(tamer.Location.X);
            WriteInt(tamer.Location.Y);
            WriteString(tamer.Name);
            WriteByte(tamer.Level);
            WriteFloat(tamer.Location.Z);
            WriteShort((short)tamer.MS);
            WriteByte(tamer.HpRate);
            WriteBytes(tamer.Equipment.ToArray());
            WriteBytes(tamer.Digivice.ToArray());
            WriteInt(tamer.CurrentCondition.GetHashCode());
            WriteInt(0); //Sync
            WriteInt(tamer.Partner.GeneralHandler);
            WriteShort(tamer.Size);
            if (tamer.Guild != null)
            {
                WriteByte(1);
                WriteInt((int)tamer.Guild.Id);
                WriteString(tamer.Guild.Name);
            }
            else
                WriteByte(0);
            WriteShort(tamer.CurrentTitle);
            WriteByte(0); //master match team
            WriteShort(tamer.SealList.SealLeaderId);
            if (tamer.CurrentCondition == Enums.ClientEnums.ConditionEnum.TamerShop)
            {
                WriteString(tamer.ShopName);
            }
            WriteInt(0);// tamer.Equipment[5].ItemId); - costume

            WriteInt(tamer.Partner.Location.X);
            WriteInt(tamer.Partner.Location.Y);
            WriteInt(tamer.Partner.GeneralHandler);
            WriteInt(tamer.Partner.CurrentType);
            WriteInt(tamer.Partner.Location.X);
            WriteInt(tamer.Partner.Location.Y);
            WriteString(tamer.Partner.Name);
            WriteShort(tamer.Partner.Size);
            WriteByte(tamer.Partner.Level);
            WriteFloat(tamer.Partner.Location.Z);
            WriteShort((short)tamer.Partner.MS);
            WriteShort((short)tamer.Partner.AS);
            WriteUInt(tamer.GeneralHandler);
            WriteByte(tamer.Partner.HpRate);
            WriteInt(tamer.Partner.CurrentCondition.GetHashCode());
            WriteShort(tamer.Partner.Digiclone.CloneLevel);
            WriteShort(tamer.Partner.Digiclone.ATLevel);
            WriteShort(tamer.Partner.Digiclone.BLLevel);
            WriteShort(tamer.Partner.Digiclone.CTLevel);
            WriteShort(0); // AS
            WriteShort(tamer.Partner.Digiclone.EVLevel);
            WriteShort(0); // HT
            WriteShort(tamer.Partner.Digiclone.HPLevel);
            WriteShort(0); // ?
            WriteShort(0); // ?

            WriteShort(0);
        }
    }
}