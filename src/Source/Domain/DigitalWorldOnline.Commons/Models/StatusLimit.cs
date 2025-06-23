using DigitalWorldOnline.Commons.Enums;

namespace DigitalWorldOnline.Commons.Models
{
    public class StatusLimit
    {
        public AccessoryTypeEnum Accessory { get; }
        public AccessoryStatusTypeEnum Status { get; }
        public byte MaxAmount { get; }

        public StatusLimit(
            AccessoryTypeEnum accType,
            AccessoryStatusTypeEnum statusType,
            byte maxAmount)
        {
            Accessory = accType;
            Status = statusType;
            MaxAmount = maxAmount;
        }
    }
}