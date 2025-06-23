using DigitalWorldOnline.Commons.Models.Events;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class BurningEventPacket : PacketWriter
    {
        private const int PacketNumber = 3132;

        /// <summary>
        /// Load the burning event.
        /// </summary>
        public BurningEventPacket(uint m_nExpRate, uint m_nNextDayExpRate, uint m_nExpTarget, uint m_nSpecialExp)
        {
            Type(PacketNumber);
            WriteUInt(1000);
            WriteUInt(100);
            WriteUInt(1);
            //WriteUInt(m_nSpecialExp);
        }
    }
}
