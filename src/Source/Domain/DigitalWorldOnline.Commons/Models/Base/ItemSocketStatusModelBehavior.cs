using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models.Base
{
    public partial class ItemSocketStatusModel
    {
        public void SetType(AccessoryStatusTypeEnum type) => Type = type;
        public void SetAttributeId(short attributeId) => AttributeId = attributeId;
        public void SetValue(short value) => Value = value;
    }
}
