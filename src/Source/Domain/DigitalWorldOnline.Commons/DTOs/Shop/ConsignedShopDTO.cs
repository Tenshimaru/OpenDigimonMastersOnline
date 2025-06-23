using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace DigitalWorldOnline.Commons.DTOs.Shop
{
    public sealed class ConsignedShopDTO : ICloneable
    {
        public long Id { get; set; }
        [MinLength(6)] public string ShopName { get; set; }
        public byte Channel { get; set; }
        public int ItemId { get; set; }
        public int GeneralHandler { get; set; }

        //Refs
        public ConsignedShopLocationDTO Location { get; set; }

        //FK
        public CharacterDTO Character { get; private set; }
        public long CharacterId { get; set; }

        public object Clone()
        {
            return (ConsignedShopDTO)MemberwiseClone();
        }

        public void SetGeneralHandler(long id = 1)
        {
            GeneralHandler = (114900 + (int)id);
        }
    }
}