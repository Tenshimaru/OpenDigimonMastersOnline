using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class HatchSpiritEvolutionPacket : PacketWriter
    {
        private const int PacketNumber = 3239;

        public HatchSpiritEvolutionPacket(int targetType,int currencyBits,List<ExtraEvolutionMaterialAssetModel> Material,List<ExtraEvolutionRequiredAssetModel> Required)
        {
            Type(PacketNumber);
            WriteInt(targetType);
            WriteInt(currencyBits);
            WriteInt(0);

            foreach (var material in Material)
            {
                WriteByte(1);
                WriteInt(material.ItemId);
  
            }

            foreach (var material in Required)
            {
                WriteByte(1);
                WriteInt(material.ItemId);
            }

            WriteByte(0);
        }
    }
}