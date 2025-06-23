using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class PartnerSwitchInBattlePacket : PacketWriter
    {
        private const int PacketNumber = 3209;

        /// <summary>
        /// Updates the character current partner.
        /// </summary>
        /// <param name="tamerModel">Tamer model.</para
        /// <param name="slot">Target item slot</param>
        public PartnerSwitchInBattlePacket(int slot, int tamerModel)
        {
            Type(PacketNumber);
            WriteInt(slot);
            WriteInt(tamerModel);
        }
    }
}