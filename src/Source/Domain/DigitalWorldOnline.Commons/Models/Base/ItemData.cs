using System.Xml.Serialization;

namespace DigitalWorldOnline.Commons.Models.Base
{
    public class ItemDataOld
    {
        [XmlAttribute("Id")]
        public int ItemId { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("HasDuration")]
        public int RemainingType { get; set; } = 0;

        [XmlElement("Mod")]
        public ushort Mod { get; set; }

        [XmlElement("Icon")]
        public string Icon { get; set; }

        [XmlElement("ItemType")]
        public short ItemType { get; set; }

        [XmlElement("Type")]
        public int Type { get; set; }

        [XmlElement("Kind")]
        public string Kind { get; set; }

        [XmlElement("Stack")]
        public short Stack { get; set; }

        [XmlElement("Cost")]
        public int Buy { get; set; }

        [XmlElement("Sell")]
        public int Sell { get; set; }

        [XmlElement("Duration")]
        public int Duration { get; set; }

        //public int uInt1 { get; set; }
        //public short[] uShorts1 { get; set; } = new short[8];
        //public short[] uShorts2 { get; set; } = new short[7];

        public ItemDataOld()
        {
            //TOTAL 8 + 12 + STACK = 23 - > VER155
            //uShorts1 = new short[8];
            //uShorts2 = new short[7];
        }
    }
}
