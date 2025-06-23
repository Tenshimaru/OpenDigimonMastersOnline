using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Utils;
using System.Text;

namespace DigitalWorldOnline.Commons.Models.Base
{
    public partial class ItemModel : ICloneable
    {
        /// <summary>
        /// Sets the current power.
        /// </summary>
        /// <param name="power">New power value</param>
        public void SetPower(byte power) => Power = power;

        /// <summary>
        /// Sets the left reroll amount.
        /// </summary>
        /// <param name="reroll">New reroll amount</param>
        public void SetReroll(byte reroll) => RerollLeft = reroll;

        public void SetFamilyType(byte familyType) => FamilyType = familyType;

        public ItemModel(int itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;

            if (Id == Guid.Empty)
                Id = Guid.NewGuid();

            if (AccessoryStatus == null || AccessoryStatus.Count != 8)
            {
                AccessoryStatus = new List<ItemAccessoryStatusModel>()
                {
                    new ItemAccessoryStatusModel(0),
                    new ItemAccessoryStatusModel(1),
                    new ItemAccessoryStatusModel(2),
                    new ItemAccessoryStatusModel(3),
                    new ItemAccessoryStatusModel(4),
                    new ItemAccessoryStatusModel(5),
                    new ItemAccessoryStatusModel(6),
                    new ItemAccessoryStatusModel(7)
                };
            }

            if (SocketStatus == null || SocketStatus.Count != 3)
            {
                SocketStatus = new List<ItemSocketStatusModel>()
                {
                    new ItemSocketStatusModel(0),
                    new ItemSocketStatusModel(1),
                    new ItemSocketStatusModel(2)
                };
            }
        }

        public uint RemainingSeconds()
        {
            if (!ItemInfo.TemporaryItem)
                return 0;

            var time = (EndDate - DateTime.Now).TotalSeconds;

            if (time <= 0)
            {
                return 0;
            }
            else
            {
                return (uint)time;
            }
        }

        public uint RemainingMinutes()
        {
            if (!ItemInfo!.TemporaryItem)
                return 0;

            if ((EndDate - DateTime.Now).TotalMinutes <= 0)
            {
                return 0xFFFFFFFF;
            }

            var time = (EndDate - DateTime.Now).TotalMinutes > 0 ? (int)(EndDate - DateTime.Now).TotalMinutes : 0;

            return (uint)time;
        }

        public uint RemainingDays()
        {
            if (!ItemInfo.TemporaryItem)
                return 0;

            var time = (EndDate - DateTime.Now).TotalDays;

            if (time <= 0)
            {
                return 0;
            }
            else
            {
                return (uint)time;
            }
        }

        /// <summary>
        /// Flags the current item if it has been expired.
        /// </summary>
        public bool Expired
        {
            get
            {
                return ItemInfo != null && ItemInfo.UseTimeType > 0 && RemainingMinutes() == 0xFFFFFFFF && FirstExpired;
            }
        }


        /// <summary>
        /// Flag for acessory status.
        /// </summary>
        public bool HasAccessoryStatus => AccessoryStatus.Any(x => x.Value > 0);

        /// <summary>
        /// Flag for socket status.
        /// </summary>
        public bool HasSocketStatus => SocketStatus.Any(x => x.Value > 0);

        /// <summary>
        /// Returns the flag with the information about item duration.
        /// </summary>
        public bool IsTemporary => ItemInfo?.UseTimeType > 0;

        /// <summary>
        /// Sets the current remaining time of the target item to the base value, if possible.
        /// </summary>
        public bool SetDefaultRemainingTime()
        {
            if (ItemInfo != null && IsTemporary)
            {
                Duration = ItemInfo.UsageTimeMinutes;
                EndDate = DateTime.Now.AddMinutes(ItemInfo.UsageTimeMinutes);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the current item id.
        /// </summary>
        /// <param name="itemId">The new item id</param>
        public void SetItemId(int itemId = 0) => ItemId = itemId;

        /// <summary>
        /// Updates the remaining time.
        /// </summary>
        /// <param name="remainingTime">The new remaining time</param>
        public void SetRemainingTime(uint remainingTime = 0)
        {
            if (remainingTime == 4294967280)
                remainingTime = 0;

            Duration = (int)remainingTime;
            EndDate = DateTime.Now.AddMinutes(remainingTime);
        }

        /// <summary>
        /// Updates the current amount.
        /// </summary>
        /// <param name="amount">The new amount</param>
        public void SetAmount(int amount = 0) => Amount = amount;

        public void SetSlot(int slot) => Slot = slot;

        public void SetTradeSlot(int slot) => TradeSlot = slot;

        /// <summary>
        /// Reduces the current amount.
        /// </summary>
        /// <param name="amount">The amount to be reduced</param>
        public void ReduceAmount(int amount) => Amount -= amount;

        /// <summary>
        /// Updates the sell price at tamer shop.
        /// </summary>
        /// <param name="sellPrice">The new sell price</param>
        public void SetSellPrice(long sellprice) => TamerShopSellPrice = sellprice;

        /// <summary>
        /// Updates the extra information about the item.
        /// </summary>
        /// <param name="info">The extra information</param>
        public void SetItemInfo(ItemAssetModel? info) => ItemInfo = info;

        public void SetFirstExpired(bool firstExpired) => FirstExpired = firstExpired;

        /// <summary>
        /// Increases the current amount.
        /// </summary>
        /// <param name="amount">The amount to be increased</param>
        public void IncreaseAmount(int amount) => Amount += amount;

        /// <summary>
        /// Serializes the current item into byte array.
        /// </summary>
        /// <returns>The serialization byte array.</returns>
        public byte[] ToArray(bool simplified = false)
        {
            byte[] buffer = Array.Empty<byte>();
            using (MemoryStream m = new())
            {
                if (ItemId > 0)
                {
                    m.Write(BitConverter.GetBytes(ItemId), 0, 4);
                    m.Write(BitConverter.GetBytes(Amount), 0, 4);

                    if (simplified)
                    {
                        m.Write(new byte[60]);
                    }
                    else
                    {
                        m.Write(BitConverter.GetBytes(0), 0, 2);
                        m.Write(BitConverter.GetBytes(0), 0, 2);
                        m.Write(BitConverter.GetBytes((short)Power), 0, 1);
                        m.Write(BitConverter.GetBytes((short)RerollLeft), 0, 1);
                        m.Write(BitConverter.GetBytes(ItemInfo.BoundType), 0, 2);

                        foreach (var socketStatus in SocketStatus.OrderBy(x => x.Slot))
                        {
                            m.Write(BitConverter.GetBytes(socketStatus.AttributeId), 0, 2);
                        }

                        foreach (var socketStatus in SocketStatus.OrderBy(x => x.Slot))
                        {
                            m.Write(BitConverter.GetBytes(socketStatus.Value), 0, 1);
                        }

                        m.Write(BitConverter.GetBytes(0), 0, 1);

                        foreach (var accessoryStatus in AccessoryStatus.OrderBy(x => x.Slot))
                        {
                            m.Write(BitConverter.GetBytes(accessoryStatus.Type.GetHashCode()), 0, 2);
                        }

                        foreach (var accessoryStatus in AccessoryStatus.OrderBy(x => x.Slot))
                        {
                            m.Write(BitConverter.GetBytes(accessoryStatus.Value), 0, 2);
                        }

                        m.Write(BitConverter.GetBytes(0), 0, 2);

                        if (RemainingMinutes() == 0) //if (RemainingMinutes() == 0xFFFFFFFF)
                        {
                            //m.Write(BitConverter.GetBytes(0xFFFFFFFF), 0, 4);
                            m.Write(BitConverter.GetBytes(0), 0, 4);
                        }
                        else
                        {
                            var ts = UtilitiesFunctions.RemainingTimeMinutes((int)RemainingMinutes());

                            m.Write(BitConverter.GetBytes(ts), 0, 4);
                        }

                        m.Write(BitConverter.GetBytes(0), 0, 4);
                    }
                }
                else
                {
                    for (int i = 0; i < GeneralSizeEnum.ItemSizeInBytes.GetHashCode(); i++)
                        m.WriteByte(0);
                }

                buffer = m.ToArray();
            }

            return buffer;
        }

        /// <summary>
        /// Serializes the current item into byte array.
        /// </summary>
        /// <returns>The serialization byte array.</returns>
        public byte[] GiftToArray(bool simplified = false)
        {
            if (ItemId <= 0)
            {
                return Array.Empty<byte>(); // Retorna um array vazio se o ItemId for menor ou igual a 0.
            }

            using (MemoryStream m = new())
            {
                m.Write(BitConverter.GetBytes(ItemId), 0, 4);
                m.Write(BitConverter.GetBytes(Amount), 0, 4);

                if (simplified)
                {
                    m.Write(new byte[60]);
                }
                else
                {
                    m.Write(BitConverter.GetBytes(0), 0, 2);
                    m.Write(BitConverter.GetBytes(0), 0, 2);
                    m.Write(BitConverter.GetBytes((short)Power), 0, 1);
                    m.Write(BitConverter.GetBytes((short)RerollLeft), 0, 1);
                    m.Write(BitConverter.GetBytes(ItemInfo.BoundType), 0, 2);
                    m.Write(BitConverter.GetBytes(0), 0, 2);
                    m.Write(BitConverter.GetBytes(0), 0, 2);
                    m.Write(BitConverter.GetBytes(0), 0, 2);
                    m.Write(BitConverter.GetBytes(0), 0, 1);
                    m.Write(BitConverter.GetBytes(0), 0, 1);
                    m.Write(BitConverter.GetBytes(0), 0, 1);
                    m.Write(BitConverter.GetBytes(0), 0, 1);

                    var orderedAccessoryStatus = AccessoryStatus.OrderBy(x => x.Slot);
                    var orderedAccessoryStatusList = orderedAccessoryStatus.ToList();

                    foreach (var accessoryStatus in orderedAccessoryStatusList)
                    {
                        m.Write(BitConverter.GetBytes(accessoryStatus.Type.GetHashCode()), 0, 2);
                    }

                    foreach (var accessoryStatus in orderedAccessoryStatusList)
                    {
                        m.Write(BitConverter.GetBytes(accessoryStatus.Value), 0, 2);
                    }

                    m.Write(BitConverter.GetBytes(0), 0, 2);
                    if (RemainingMinutes() == 4294967280)
                    {
                        m.Write(BitConverter.GetBytes(RemainingMinutes()), 0, 4);
                    }
                    else
                    {
                        m.Write(BitConverter.GetBytes(UtilitiesFunctions.RemainingTimeMinutes((int)RemainingMinutes())),
                            0, 4);
                    }

                    m.Write(BitConverter.GetBytes(0), 0, 4);
                }

                return m.ToArray();
            }
        }

        public byte[] NewGiftToArray()
        {
            if (ItemId <= 0)
            {
                return Array.Empty<byte>();
            }

            using (MemoryStream m = new())
            {
                m.Write(BitConverter.GetBytes(ItemId), 0, 4);
                m.Write(BitConverter.GetBytes(Amount), 0, 4);

                m.Write(BitConverter.GetBytes(0), 0, 2);
                m.Write(BitConverter.GetBytes(0), 0, 2);
                m.Write(BitConverter.GetBytes((short)Power), 0, 1);
                m.Write(BitConverter.GetBytes((short)RerollLeft), 0, 1);

                if (ItemInfo != null)
                {
                    m.Write(BitConverter.GetBytes(ItemInfo.BoundType), 0, 2);
                }
                else
                {
                    m.Write(BitConverter.GetBytes((short)0), 0, 2);
                }

                m.Write(BitConverter.GetBytes(0), 0, 2);
                m.Write(BitConverter.GetBytes(0), 0, 2);
                m.Write(BitConverter.GetBytes(0), 0, 2);
                m.Write(BitConverter.GetBytes(0), 0, 1);
                m.Write(BitConverter.GetBytes(0), 0, 1);
                m.Write(BitConverter.GetBytes(0), 0, 1);
                m.Write(BitConverter.GetBytes(0), 0, 1);

                var orderedAccessoryStatus = AccessoryStatus.OrderBy(x => x.Slot);
                var orderedAccessoryStatusList = orderedAccessoryStatus.ToList();

                foreach (var accessoryStatus in orderedAccessoryStatusList)
                {
                    m.Write(BitConverter.GetBytes(accessoryStatus.Type.GetHashCode()), 0, 2);
                }

                foreach (var accessoryStatus in orderedAccessoryStatusList)
                {
                    m.Write(BitConverter.GetBytes(accessoryStatus.Value), 0, 2);
                }

                m.Write(BitConverter.GetBytes(0), 0, 2);

                long nEndTime = (uint)new DateTimeOffset(EndDate).ToUnixTimeSeconds();

                m.Write(BitConverter.GetBytes(nEndTime), 0, 4);

                m.Write(BitConverter.GetBytes(0), 0, 4);

                return m.ToArray();
            }
        }

        public string ToString()
        {
            var sb = new StringBuilder();

            if (ItemId > 0)
            {
                sb.AppendLine($"Amount {Amount.ToString()}");
                sb.AppendLine($"Power {Power.ToString()}");
                sb.AppendLine($"RerollLeft {RerollLeft.ToString()}");
                sb.AppendLine($"BoundType {ItemInfo?.BoundType.ToString()}");

                foreach (var accessoryStatus in AccessoryStatus.OrderBy(x => x.Slot))
                {
                    sb.AppendLine($"AccessoryStatus{accessoryStatus.Slot}");
                    sb.AppendLine($"Type {accessoryStatus.Type.ToString()}");
                    sb.AppendLine($"Value {accessoryStatus.Value.ToString()}");
                }

                //sb.AppendLine($"RemainingTime {RemainingTime.ToString()}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the current amount of the target status type.
        /// </summary>
        /// <param name="type">Target status type</param>
        public byte StatusAmount(AccessoryStatusTypeEnum type) => (byte)AccessoryStatus.Count(x => x.Type == type);

        /// <summary>
        /// Clon's an item properties, but keeps the same identifier.
        /// </summary>
        /// <param name="id">The identifier that will be kept.</param>
        /// <returns>The cloned item with the source identifier.</returns>
        public object Clone(Guid id)
        {
            var clonedObject = (ItemModel)Clone();
            clonedObject.Id = id;

            return clonedObject;
        }

        /// <summary>
        /// Clon's an item properties.
        /// </summary>
        /// <returns>The cloned item.</returns>
        public object Clone()
        {
            return (ItemModel)MemberwiseClone();
        }
    }
}