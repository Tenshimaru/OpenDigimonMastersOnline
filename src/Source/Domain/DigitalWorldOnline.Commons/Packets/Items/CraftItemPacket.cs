using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.Items
{
    public class CraftItemPacket : PacketWriter
    {
        private const int PacketNumber = 3982;

        public CraftItemPacket(ItemCraftAssetModel craftRecipe, int requestAmount, int finalAmount)
        {
            Type(PacketNumber);
            WriteInt(0);
            WriteInt(craftRecipe.ItemId);
            WriteInt(requestAmount);
            WriteInt(0);
            WriteInt(0);
            WriteInt(0);
            WriteInt(requestAmount);
            WriteInt(finalAmount);
            WriteInt(craftRecipe.Materials.Count);

            foreach (var material in craftRecipe.Materials)
            {
                WriteInt(material.ItemId);
                WriteInt(material.Amount * requestAmount);
            }

            WriteInt(0);
        }
    }
}