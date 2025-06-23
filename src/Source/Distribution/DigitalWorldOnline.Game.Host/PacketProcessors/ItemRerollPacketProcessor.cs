using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ItemRerollPacketProcessor : IGamePacketProcessor
    {
        private static readonly List<int> _normalStone = new()
        {
            45003, 46996, 46998, 47003, 47007
        };

        private static readonly List<int> _advancedStone = new()
        {
            47008 , 47106
        };

        private static readonly List<int> _digitaryStone = new()
        {
            45000, 47000
        };

        private static readonly List<int> _highStone = new()
        {
            46997, 46999
        };

        private static readonly List<int> _shinyStone = new()
        {
            10026, 47004
        };

        private static readonly List<int> _amazingStone = new()
        {
            10259, 47009, 47107
        };

        private static readonly List<int> _optionNumberChange = new()
        {
            10052, 45001, 47001, 47005
        };

        private static readonly List<int> _numberChange = new()
        {
            10053, 45002, 47002, 47006
        };

        private readonly List<StatusLimit> StatusLimit;

        public GameServerPacketEnum Type => GameServerPacketEnum.ItemReroll;

        private readonly AssetsLoader _assets;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public ItemRerollPacketProcessor(
            AssetsLoader assets,
            ISender sender,
            ILogger logger)
        {
            _assets = assets;
            _sender = sender;
            _logger = logger;

            StatusLimit = new List<StatusLimit>()
            {
                new StatusLimit(AccessoryTypeEnum.Ring, AccessoryStatusTypeEnum.HP, 3),
                new StatusLimit(AccessoryTypeEnum.Ring, AccessoryStatusTypeEnum.DS, 3),
                new StatusLimit(AccessoryTypeEnum.Ring, AccessoryStatusTypeEnum.AT, 2),
                new StatusLimit(AccessoryTypeEnum.Ring, AccessoryStatusTypeEnum.CT, 2),
                new StatusLimit(AccessoryTypeEnum.Ring, AccessoryStatusTypeEnum.DE, 2),
                new StatusLimit(AccessoryTypeEnum.Ring, AccessoryStatusTypeEnum.ATT, 2),
                new StatusLimit(AccessoryTypeEnum.Ring, AccessoryStatusTypeEnum.SCD, 2),

                new StatusLimit(AccessoryTypeEnum.Necklace, AccessoryStatusTypeEnum.HP, 3),
                new StatusLimit(AccessoryTypeEnum.Necklace, AccessoryStatusTypeEnum.DS, 3),
                new StatusLimit(AccessoryTypeEnum.Necklace, AccessoryStatusTypeEnum.AT, 1),
                new StatusLimit(AccessoryTypeEnum.Necklace, AccessoryStatusTypeEnum.AS, 1),
                new StatusLimit(AccessoryTypeEnum.Necklace, AccessoryStatusTypeEnum.CD, 1),
                new StatusLimit(AccessoryTypeEnum.Necklace, AccessoryStatusTypeEnum.CT, 2),
                new StatusLimit(AccessoryTypeEnum.Necklace, AccessoryStatusTypeEnum.DE, 2),
                new StatusLimit(AccessoryTypeEnum.Necklace, AccessoryStatusTypeEnum.ATT, 2),
                new StatusLimit(AccessoryTypeEnum.Necklace, AccessoryStatusTypeEnum.SCD, 1),

                new StatusLimit(AccessoryTypeEnum.Earring, AccessoryStatusTypeEnum.HP, 2),
                new StatusLimit(AccessoryTypeEnum.Earring, AccessoryStatusTypeEnum.DS, 2),
                new StatusLimit(AccessoryTypeEnum.Earring, AccessoryStatusTypeEnum.HT, 1),
                new StatusLimit(AccessoryTypeEnum.Earring, AccessoryStatusTypeEnum.BL, 1),
                new StatusLimit(AccessoryTypeEnum.Earring, AccessoryStatusTypeEnum.CD, 2),
                new StatusLimit(AccessoryTypeEnum.Earring, AccessoryStatusTypeEnum.CT, 1),
                new StatusLimit(AccessoryTypeEnum.Earring, AccessoryStatusTypeEnum.DE, 2),
                new StatusLimit(AccessoryTypeEnum.Earring, AccessoryStatusTypeEnum.ATT, 2),
                new StatusLimit(AccessoryTypeEnum.Earring, AccessoryStatusTypeEnum.SCD, 1),
                new StatusLimit(AccessoryTypeEnum.Earring, AccessoryStatusTypeEnum.EV, 1),

                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.HP, 2),
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.DS, 2),
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.AT, 1),
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.DE, 1),
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.CD, 2),
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.AS, 1),
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.CT, 2),
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.BL, 2),
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.SCD, 1),
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.HT, 2),
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.EV, 2),

                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.HP, 2),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.DS, 2),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.AT, 2),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.DE, 2),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.AS, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.HT, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.CD, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.ATT, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Data, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Vacina, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Virus, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Unknown, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Ice, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Water, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Fire, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Earth, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Wind, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Wood, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Light, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Dark, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Thunder, 1),
                new StatusLimit(AccessoryTypeEnum.Digivice, AccessoryStatusTypeEnum.Steel, 1)
            };
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var tamerHandle = packet.ReadInt();
            var consumableSlot = packet.ReadShort();
            var accessorySlot = packet.ReadShort();
            var changedStatusSlot = packet.ReadByte();

            var consumedStone = client.Tamer.Inventory.FindItemBySlot(consumableSlot);
            if (consumedStone == null || consumedStone.ItemId == 0)
            {
                _logger.Warning($"Invalid item at slot {consumableSlot} for tamer {client.TamerId}.");
                client.Send(UtilitiesFunctions.GroupPackets(
                        new SystemMessagePacket($"Invalid stone at slot {consumableSlot}.").Serialize(),
                        new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                return;
            }

            var targetAccessory = client.Tamer.Inventory.FindItemBySlot(accessorySlot);
            if (targetAccessory == null || targetAccessory.ItemId == 0)
            {
                _logger.Warning($"Invalid item at slot {accessorySlot} for tamer {client.TamerId}.");
                client.Send(UtilitiesFunctions.GroupPackets(
                        new SystemMessagePacket($"Invalid accessory at slot {accessorySlot}.").Serialize(),
                        new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                return;
            }

            byte result = 1; //1 = success | 2 = fail | 3 = dont change

            if (_normalStone.Contains(consumedStone.ItemId))
            {
                if (targetAccessory.RerollLeft >= 20)
                {
                    result = 3;

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                    _logger.Verbose($"Character {client.TamerId} cannot increase item {targetAccessory.ItemId} reroll left with {consumedStone.ItemId}.");
                    return;
                }
                else
                {
                    result = UtilitiesFunctions.RandomByte(0, 1);

                    targetAccessory.RerollLeft += result;

                    client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));

                    if (result > 0)
                        _logger.Verbose($"Character {client.TamerId} increased item {targetAccessory.ItemId} reroll left by {result} with {consumedStone.ItemId}.");
                    else
                        _logger.Verbose($"Character {client.TamerId} failed to increase item {targetAccessory.ItemId} reroll left with {consumedStone.ItemId}.");
                }
            }

            if (_advancedStone.Contains(consumedStone.ItemId))
            {
                if (targetAccessory.RerollLeft + 5 > 20)
                {
                    result = 3;

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                    _logger.Verbose($"Character {client.TamerId} cannot increase item {targetAccessory.ItemId} reroll left with {consumedStone.ItemId}.");
                    return;
                }
                else
                {
                    targetAccessory.RerollLeft += 5;

                    client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                    _logger.Verbose($"Character {client.TamerId} increased item {targetAccessory.ItemId} reroll left by 5 with {consumedStone.ItemId}.");
                }
            }

            if (_digitaryStone.Contains(consumedStone.ItemId))
            {
                if (targetAccessory.Power >= 200)
                {
                    result = 3;

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                    _logger.Verbose($"Character {client.TamerId} cannot increase item {targetAccessory.ItemId} power with {consumedStone.ItemId}.");
                    return;
                }
                else
                {
                    result = UtilitiesFunctions.RandomByte(1, 3);

                    if (result == 1)
                    {
                        targetAccessory.Power += 1;
                        _logger.Verbose($"Character {client.TamerId} increase item {targetAccessory.ItemId} power by 1 with {consumedStone.ItemId}.");
                    }
                    else if (result == 2)
                    {
                        targetAccessory.Power -= 1;
                        _logger.Verbose($"Character {client.TamerId} decreased item {targetAccessory.ItemId} power by 1 with {consumedStone.ItemId}.");
                    }
                    else
                    {
                        _logger.Verbose($"Character {client.TamerId} item {targetAccessory.ItemId} power has no changes with {consumedStone.ItemId}.");
                    }

                    client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                }
            }

            if (_highStone.Contains(consumedStone.ItemId))
            {
                if (targetAccessory.Power + 2 > 200)
                {
                    result = 3;

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                    _logger.Verbose($"Character {client.TamerId} cannot increase item {targetAccessory.ItemId} power with {consumedStone.ItemId}.");
                    return;
                }
                else
                {
                    result = UtilitiesFunctions.RandomByte(1, 3);

                    if (result == 1)
                    {
                        targetAccessory.Power += 2;
                        _logger.Verbose($"Character {client.TamerId} increase item {targetAccessory.ItemId} power by 2 with {consumedStone.ItemId}.");
                    }
                    else if (result == 2)
                    {
                        targetAccessory.Power -= 2;
                        _logger.Verbose($"Character {client.TamerId} decreased item {targetAccessory.ItemId} power by 2 with {consumedStone.ItemId}.");
                    }
                    else
                    {
                        _logger.Verbose($"Character {client.TamerId} item {targetAccessory.ItemId} power has no changes with {consumedStone.ItemId}.");
                    }

                    client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                }
            }

            if (_shinyStone.Contains(consumedStone.ItemId))
            {
                if (targetAccessory.Power + 3 > 200)
                {
                    result = 3;

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));

                    _logger.Verbose($"Character {client.TamerId} cannot increase item {targetAccessory.ItemId} power with {consumedStone.ItemId}.");
                    return;
                }
                else
                {
                    targetAccessory.Power += 3;

                    client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));

                    _logger.Verbose($"Character {client.TamerId} increase item {targetAccessory.ItemId} power by 3 with {consumedStone.ItemId}.");
                }
            }

            if (_amazingStone.Contains(consumedStone.ItemId))
            {
                if (targetAccessory.Power + 10 > 200)
                {
                    result = 3;

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));

                    _logger.Verbose($"Character {client.TamerId} cannot increase item {targetAccessory.ItemId} power with {consumedStone.ItemId}.");
                    return;
                }
                else
                {
                    targetAccessory.Power += 10;

                    client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));

                    _logger.Verbose($"Character {client.TamerId} increase item {targetAccessory.ItemId} power by 10 with {consumedStone.ItemId}.");
                }
            }

            if (_optionNumberChange.Contains(consumedStone.ItemId))
            {
                result = 0;

                if (targetAccessory.RerollLeft == 0)
                {
                    result = 3;

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));

                    _logger.Debug($"Character {client.TamerId} cannot reroll item {targetAccessory.ItemId} status with {consumedStone.ItemId}.");
                    return;
                }
                else
                {
                    targetAccessory.RerollLeft -= 1;

                    var itemInfo = _assets.ItemInfo.FirstOrDefault(x => x.ItemId == targetAccessory.ItemId);
                    if (itemInfo == null)
                    {
                        result = 3;

                        client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                        client.Send(UtilitiesFunctions.GroupPackets(
                                new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                                new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize(),
                                new SystemMessagePacket($"Invalid item info for item {targetAccessory.ItemId}.").Serialize()));

                        _logger.Warning($"Invalid item info for item {targetAccessory.ItemId} and tamer {client.TamerId}.");
                        return;
                    }

                    var accessoryAsset = _assets.AccessoryRoll.FirstOrDefault(x => x.ItemId == targetAccessory.ItemId);
                    if (accessoryAsset != null)
                    {
                        targetAccessory.AccessoryStatus = targetAccessory.AccessoryStatus.OrderBy(x => x.Slot).ToList();

                        for (int i = 0; i < accessoryAsset.StatusAmount; i++)
                        {
                            var forbiddenStatusType = new List<AccessoryStatusTypeEnum>();

                            if (accessoryAsset.StatusAmount > 1)
                            {
                                foreach (AccessoryStatusTypeEnum statusType in Enum.GetValues(typeof(AccessoryStatusTypeEnum)))
                                {
                                    var currentAmount = targetAccessory.StatusAmount(statusType);

                                    if (currentAmount >= StatusLimit.FirstOrDefault(x => x.Accessory == (AccessoryTypeEnum)itemInfo.Type && x.Status == statusType)?.MaxAmount)
                                    {
                                        forbiddenStatusType.Add(statusType);
                                    }
                                }
                            }

                            var possibleStatus = accessoryAsset.Status.Where(x => !forbiddenStatusType.Contains((AccessoryStatusTypeEnum)x.Type));

                            var newStatus = possibleStatus.OrderBy(x => Guid.NewGuid()).First();

                            targetAccessory.AccessoryStatus[i].SetType((AccessoryStatusTypeEnum)newStatus.Type);
                            targetAccessory.AccessoryStatus[i].SetValue(UtilitiesFunctions.RandomShort((short)newStatus.MinValue, (short)(newStatus.MaxValue)));
                        }

                        var statusString = targetAccessory.AccessoryStatus.Where(x => x.Value > 0)?.Select(x => $"{x.Type} {x.Value}");
                        _logger.Verbose($"Character {client.TamerId} rerolled status for item {targetAccessory.ItemId} with {consumedStone.ItemId} power {targetAccessory.Power} " +
                            $"reroll {targetAccessory.RerollLeft} and new status {string.Join(',', statusString)}.");

                        client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                        client.Send(UtilitiesFunctions.GroupPackets(
                                new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                                new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                    }
                    else
                    {
                        result = 3;

                        //client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                        client.Send(UtilitiesFunctions.GroupPackets(
                                new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                                new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize(),
                                new SystemMessagePacket($"Invalid accessory asset for item {targetAccessory.ItemId}.").Serialize()));

                        _logger.Warning($"Invalid accessory asset with item id {targetAccessory.ItemId} for tamer {client.TamerId}.");
                        return;
                    }
                }
            }

            if (_numberChange.Contains(consumedStone.ItemId))
            {
                result = 0;

                if (targetAccessory.RerollLeft == 0)
                {
                    result = 3;

                    client.Send(UtilitiesFunctions.GroupPackets(
                            new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));

                    _logger.Debug($"Character {client.TamerId} cannot reroll item {targetAccessory.ItemId} status with {consumedStone.ItemId}.");
                    return;
                }
                else
                {
                    targetAccessory.RerollLeft -= 1;

                    var accessoryAsset = _assets.AccessoryRoll.FirstOrDefault(x => x.ItemId == targetAccessory.ItemId);
                    if (accessoryAsset != null)
                    {
                        targetAccessory.AccessoryStatus = targetAccessory.AccessoryStatus.OrderBy(x => x.Slot).ToList();

                        var newStatus = accessoryAsset.Status.FirstOrDefault(x => (AccessoryStatusTypeEnum)x.Type == targetAccessory.AccessoryStatus[changedStatusSlot].Type);

                        if (newStatus == null)
                        {
                            result = 3;

                            client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                            client.Send(UtilitiesFunctions.GroupPackets(
                                    new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                                    new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize(),
                                    new SystemMessagePacket($"Invalid accessory status reroll info for item {accessoryAsset.ItemId}.").Serialize()));

                            _logger.Warning($"Invalid accessory status for asset {accessoryAsset.Id} while tamer {client.TamerId} reroll.");
                            return;
                        }

                        targetAccessory.AccessoryStatus[changedStatusSlot].SetValue(UtilitiesFunctions.RandomShort((short)newStatus.MinValue, (short)(newStatus.MaxValue)));

                        client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                        var statusString = targetAccessory.AccessoryStatus.Where(x => x.Value > 0)?.Select(x => $"{x.Type} {x.Value}");

                        client.Send(UtilitiesFunctions.GroupPackets(
                                new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                                new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()));
                    }
                    else
                    {
                        result = 3;

                        //client.Tamer.Inventory.RemoveOrReduceItem(consumedStone, 1);

                        client.Send(UtilitiesFunctions.GroupPackets(
                                new ItemRerollPacket(result, accessorySlot, targetAccessory).Serialize(),
                                new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize(),
                                new SystemMessagePacket($"Invalid accessory asset for item {targetAccessory.ItemId}.").Serialize()));

                        _logger.Warning($"Invalid accessory asset with item id {targetAccessory.ItemId} for tamer {client.TamerId}.");
                        return;
                    }
                }
            }

            await _sender.Send(new UpdateItemAccessoryStatusCommand(targetAccessory));
            await _sender.Send(new UpdateItemCommand(consumedStone));
        }
    }
}