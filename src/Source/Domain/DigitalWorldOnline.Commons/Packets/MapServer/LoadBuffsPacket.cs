
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.MapServer
{
    public class LoadBuffsPacket : PacketWriter
    {
        private const int PacketNumber = 1006;

   
        public LoadBuffsPacket(CharacterModel characterModel)
        {
            Type(PacketNumber);
            WriteByte(16);
            WriteShort(1);
            WriteInt(characterModel.GeneralHandler);
            WriteByte((byte)characterModel.BuffList.ActiveBuffs.Count);
            foreach (var buff in characterModel.BuffList.ActiveBuffs)
            {
                WriteUShort((ushort)buff.BuffId);
                WriteShort(1);
                WriteInt(Utils.UtilitiesFunctions.RemainingTimeSeconds(buff.RemainingSeconds));
                WriteInt(buff.SkillId);
            }

            WriteShort(1);
            WriteInt(characterModel.Partner.GeneralHandler);
            WriteByte((byte)(characterModel.Partner.BuffList.ActiveBuffs.Count + (byte)characterModel.Partner.DebuffList.ActiveBuffs.Count));

            foreach (var buff in characterModel.Partner.BuffList.ActiveBuffs)
            {
                WriteUShort((ushort)buff.BuffId);
                WriteShort(1);
                WriteInt(Utils.UtilitiesFunctions.RemainingTimeSeconds(buff.RemainingSeconds));
                WriteInt(buff.SkillId);
            }

            foreach (var buff in characterModel.Partner.DebuffList.ActiveBuffs)
            {
                WriteUShort((ushort)buff.BuffId);
                WriteShort(1);
                WriteInt(Utils.UtilitiesFunctions.RemainingTimeSeconds(buff.RemainingSeconds));
                WriteInt(buff.SkillId);
            }
            WriteShort(0);
            WriteByte(0);
        }

        public LoadBuffsPacket(MobConfigModel mob)
        {
            Type(PacketNumber);
            WriteByte(16);
            WriteShort(0);       
            WriteShort(0);
            WriteShort(1);
            WriteInt(mob.GeneralHandler);
            WriteByte((byte)mob.DebuffList.ActiveBuffs.Count);

            foreach (var buff in mob.DebuffList.ActiveBuffs)
            {
                WriteUShort((ushort)buff.BuffId);
                WriteShort(1);
                WriteInt(Utils.UtilitiesFunctions.RemainingTimeSeconds(buff.RemainingSeconds));
                WriteInt(buff.SkillId);
            }

            WriteByte(0);
        }
    }
}