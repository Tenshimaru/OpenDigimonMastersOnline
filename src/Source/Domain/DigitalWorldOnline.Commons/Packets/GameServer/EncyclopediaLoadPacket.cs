using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;
using System.Collections.Generic;
using static System.Reflection.Metadata.BlobBuilder;

namespace DigitalWorldOnline.Commons.Packets.GameServer
{
    public class EncyclopediaLoadPacket : PacketWriter
    {
        private const int PacketNumber = 3234;

        public EncyclopediaLoadPacket(List<CharacterEncyclopediaModel> Encyclopedia)
        {
            Type(PacketNumber);

            WriteInt(Encyclopedia.Count);
            Encyclopedia.ForEach(encyclopediaRecord =>
            {
                long nSlotOpened = 0;

                encyclopediaRecord.Evolutions.Select((s, i) => new { s, i }).ToDictionary(x => x.i, x => x.s)
                .ToList()
                .ForEach(x =>
                {
                    if (x.Value.IsUnlocked)
                    {
                        nSlotOpened |= (1L << x.Key);
                    }
                });

                var isRewardNotAllowed = encyclopediaRecord is { IsRewardReceived: true, IsRewardAllowed: false };
                WriteInt(encyclopediaRecord.EvolutionAsset.Type);
                WriteShort(encyclopediaRecord.Level);
                WriteInt64(nSlotOpened);
                WriteShort(encyclopediaRecord.EnchantAT);
                WriteShort(encyclopediaRecord.EnchantBL);
                WriteShort(encyclopediaRecord.EnchantCT);
                WriteShort(encyclopediaRecord.EnchantEV);
                WriteShort(encyclopediaRecord.EnchantHP);

                WriteShort(encyclopediaRecord.Size);
                WriteByte(Convert.ToByte(isRewardNotAllowed));
            });
            WriteByte(0);
        }

    }
}