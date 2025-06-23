using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;
using System;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class RareMachineRunPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.RareMachineRun;

        private readonly MapServer _mapServer;
        private readonly ILogger _logger;
        private readonly AssetsLoader _assets;
        private readonly ISender _sender;


        public RareMachineRunPacketProcessor(
            MapServer mapServer,
            AssetsLoader assets,
            ILogger logger,
            ISender sender)
        {
            _mapServer = mapServer;
            _assets = assets;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var GeneralHandler = packet.ReadInt();
            var NpcId = packet.ReadInt();
            var inventorySlot = packet.ReadInt();
            var NpcId2 = packet.ReadInt();

            var Kind = 1;

            Random random = new Random();

            var Gotcha = _assets.Gotcha.FirstOrDefault(x => x.NpcId == NpcId);

            var ItemInfo = client.Tamer.Inventory.FindItemBySlot(inventorySlot);

            if (ItemInfo.Amount <= 0)
            {
                client.Send(new GotchaErrorPacket());
                client.Disconnect();
            }
            else
            {
                var availableItems = Gotcha.Items.Where(item => item.Quanty > 0).ToList();
                var availableRareItems = Gotcha.RareItems.Where(item => item.RareItemCnt > 0).ToList();

                if (availableItems.Count < 1 && availableRareItems.Count < 1)
                {

                    foreach (var normalItem in Gotcha.Items) normalItem.Quanty = normalItem.InitialQuanty;
                    foreach (var rareItem in Gotcha.RareItems) rareItem.RareItemCnt = rareItem.RareItemGive;

                    client.Send(new GotchaErrorPacket());
                    return;
                }
                client.Tamer.Inventory.RemoveOrReduceItem(ItemInfo, Gotcha.UseCount, inventorySlot);
                if (random.Next(0, 101) > Gotcha.Chance || availableRareItems.Count < 1)
                {

                    int index = random.Next(availableItems.Count); // Gera um índice aleatório
                    var Item = availableItems[index].ItemId;
                    var Quanty = availableItems[index].ItemCount;
                    //Reduce Quanty
                    availableItems[index].Quanty -= 1;
                    client.Send(new GotchaRunPacket(Gotcha, Kind, Item, Quanty));

                    var newItem = new ItemModel();
                    newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == Item));

                    newItem.ItemId = Item;
                    newItem.Amount = Quanty;

                    if (newItem.IsTemporary)
                        newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                    var itemClone = (ItemModel)newItem.Clone();
                    if (client.Tamer.Inventory.AddItem(newItem))
                    {
                        await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                    }
                }
                else
                {
                    Kind = 0;
                    int index = random.Next(availableRareItems.Count); // Gera um índice aleatório
                    var Item = availableRareItems[index].RareItem;
                    var Quanty = 1;
                    //Reduce Quanty
                    availableRareItems[index].RareItemCnt -= (short)Quanty;
                    client.Send(new GotchaRunPacket(Gotcha, Kind, Item, Quanty));
                    var newItem = new ItemModel();
                    newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == Item));

                    newItem.ItemId = Item;
                    newItem.Amount = Quanty;

                    if (newItem.IsTemporary)
                        newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                    var itemClone = (ItemModel)newItem.Clone();
                    if (client.Tamer.Inventory.AddItem(newItem))
                    {
                        await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                    }
                }
                await _sender.Send(new UpdateItemCommand(ItemInfo));
            }
        }
    }
}
