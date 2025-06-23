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
    public class ItemIdentifyPacketProcessor : IGamePacketProcessor
    {
        private readonly List<StatusLimit> StatusLimit;

        public GameServerPacketEnum Type => GameServerPacketEnum.ItemIdentify;

        private readonly AssetsLoader _assets;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public ItemIdentifyPacketProcessor(
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
                new StatusLimit(AccessoryTypeEnum.Bracelet, AccessoryStatusTypeEnum.CD, 2),
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

            var tamerhandle = packet.ReadInt();
            var slot = packet.ReadShort();

            var identifiedItem = client.Tamer.Inventory.FindItemBySlot(slot);

            if (identifiedItem != null)
            {
                var itemInfo = _assets.ItemInfo.FirstOrDefault(x => x.ItemId == identifiedItem.ItemId);
                if (itemInfo == null)
                {
                    _logger.Error($"Invalid item info for item {identifiedItem.ItemId} and tamer {client.TamerId}.");
                    client.Send(new SystemMessagePacket($"Invalid item info for item {identifiedItem.ItemId}."));
                    return;
                }

                var accessoryAsset = _assets.AccessoryRoll.FirstOrDefault(x => x.ItemId == identifiedItem.ItemId);
                if (accessoryAsset != null)
                {
                    identifiedItem.AccessoryStatus = identifiedItem.AccessoryStatus.OrderBy(x => x.Slot).ToList();

                    for (int i = 0; i < accessoryAsset.StatusAmount; i++)
                    {
                        var forbiddenStatusType = new List<AccessoryStatusTypeEnum>();

                        if (accessoryAsset.StatusAmount > 1)
                        {
                            foreach (AccessoryStatusTypeEnum statusType in Enum.GetValues(typeof(AccessoryStatusTypeEnum)))
                            {
                                var currentAmount = identifiedItem.StatusAmount(statusType);

                                if (currentAmount >= StatusLimit.FirstOrDefault(x => x.Accessory == (AccessoryTypeEnum)itemInfo.Type && x.Status == statusType)?.MaxAmount)
                                {
                                    forbiddenStatusType.Add(statusType);
                                }
                            }
                        }

                        var possibleStatus = accessoryAsset.Status.Where(x => !forbiddenStatusType.Contains((AccessoryStatusTypeEnum)x.Type));

                        var newStatus = possibleStatus.OrderBy(x => Guid.NewGuid()).First();

                        identifiedItem.AccessoryStatus[i].SetType((AccessoryStatusTypeEnum)newStatus.Type);
                        identifiedItem.AccessoryStatus[i].SetValue(UtilitiesFunctions.RandomShort((short)newStatus.MinValue, (short)(newStatus.MaxValue)));
                    }

                    identifiedItem.SetPower(UtilitiesFunctions.RandomByte(95, 102));
                    identifiedItem.SetReroll((byte)accessoryAsset.RerollAmount);

                    await _sender.Send(new UpdateItemAccessoryStatusCommand(identifiedItem));

                    var statusString = identifiedItem.AccessoryStatus.Where(x => x.Value > 0)?.Select(x => $"{x.Type} {x.Value}");
                    _logger.Verbose($"Character {client.TamerId} identified item {identifiedItem.ItemId} with power {identifiedItem.Power} " +
                        $"reroll {identifiedItem.RerollLeft} and status {string.Join(',', statusString)}.");

                    client.Send(
                        UtilitiesFunctions.GroupPackets(
                            new ItemIdentifyPacket(slot, identifiedItem).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()
                        )
                    );
                }
                else
                {
                    _logger.Error($"Invalid accessory asset with item id {identifiedItem.ItemId} for tamer {client.TamerId}.");
                    client.Send(new SystemMessagePacket($"Invalid accessory asset."));
                }
            }
            else
            {
                _logger.Error($"Invalid item for accessory identify at slot {slot} for tamer {client.TamerId}.");
                client.Send(new SystemMessagePacket($"Invalid item."));
            }
        }
    }
}