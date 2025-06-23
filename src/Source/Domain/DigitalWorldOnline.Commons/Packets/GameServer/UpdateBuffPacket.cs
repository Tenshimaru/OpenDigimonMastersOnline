using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class UpdateBuffPacket : PacketWriter
    {
        private const int PacketNumber = 4001;

        /// <summary>
        /// Updates an active buff.
        /// </summary>
        /// <param name="handler">Target handler</param>
        /// <param name="buff">Target buff information</param>
        /// <param name="duration">Buff duration</param>
        public UpdateBuffPacket(int handler, BuffInfoAssetModel buff, short TypeN,int duration)
        {
            Type(PacketNumber);
            WriteUInt((uint)handler);
            WriteShort((short)buff.BuffId);
            WriteShort(TypeN);
            WriteInt(duration);
            WriteInt(buff.SkillCode);

            //WriteUInt((uint)handler);
            //WriteShort((short)buff.BuffId);
            //WriteShort(buff.EffectValue);
            //WriteInt(duration);
            //WriteInt(buff.SkillCode);
        }
    }
}