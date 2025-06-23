using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class SealsPacket : PacketWriter
    {
        private const int PacketNumber = 1333;

        /// <summary>
        /// Load the tamer's current seals.
        /// </summary>
        /// <param name="sealList">The tamer's seals to load</param>
        public SealsPacket(CharacterSealListModel sealList)
        {
            Type(PacketNumber);
            WriteShort(sealList.SealLeaderId);

            WriteShort((short)sealList.Seals.Count);
            foreach (var seal in sealList.Seals)
            {
                WriteShort(0);
                WriteInt(seal.SealId);
                WriteShort(seal.Amount);
            }

            WriteShort((short)sealList.Seals.Count);
            foreach (var seal in sealList.Seals)
            {
                WriteShort(seal.SequentialId);
                WriteByte(Convert.ToByte(seal.Favorite));
            }
        }
    }
}