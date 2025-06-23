using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class MembershipPacket : PacketWriter
    {
        private const int PacketNumber = 3414;

        /// <summary>
        /// Load the account remaining membership time.
        /// </summary>
        public MembershipPacket(DateTime membershipExpirationDate, int utcSeconds)
        {
            var secondsUTC = (membershipExpirationDate - DateTime.UtcNow).TotalSeconds;

            if (utcSeconds <= 0)
                utcSeconds = 0;
            
            Type(PacketNumber);
            WriteByte(Convert.ToByte(secondsUTC > 0));
            WriteInt(utcSeconds);
        }

        public MembershipPacket()
        {
            Type(PacketNumber);
            WriteByte(0);
            WriteInt(0);
        }
    }
}