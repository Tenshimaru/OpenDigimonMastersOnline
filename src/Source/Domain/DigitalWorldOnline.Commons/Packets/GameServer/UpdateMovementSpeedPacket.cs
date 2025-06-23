using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class UpdateMovementSpeedPacket : PacketWriter
    {
        private const int PacketNumber = 9905;

        /// <summary>
        /// Update the tamer and partner movement speed.
        /// </summary>
        /// <param name="character">The server character</param>
        public UpdateMovementSpeedPacket(CharacterModel character)
        {
            Type(PacketNumber);
            WriteUInt(character.GeneralHandler);
            WriteUInt(character.Partner.GeneralHandler);

            if (character.CurrentCondition == ConditionEnum.Ride)
            {
                WriteShort((short)(character.ProperMS * 2));
                WriteShort((short)(character.ProperMS * 2));
            }
            else
            {
                WriteShort(character.ProperMS);
                WriteShort(character.ProperMS);
            }

            WriteInt(character.CurrentCondition.GetHashCode());
            WriteInt(character.Partner.CurrentCondition.GetHashCode());
        }
    }
}
