using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class NpcPurchasePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.NpcItemPurchase;

        private readonly AssetsLoader _assets;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public NpcPurchasePacketProcessor(
            AssetsLoader assets,
            ISender sender,
            ILogger logger)
        {
            _assets = assets;
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var npcId = packet.ReadInt();
            var unk = packet.ReadByte();
            var shopSlot = packet.ReadInt();
            var purchaseCount = packet.ReadShort();

            var npc = _assets.Npcs.FirstOrDefault(x => x.NpcId == npcId && x.MapId == client.Tamer.Location.MapId);

            //_logger.Information($"NpcId: {npcId} | unk: {unk}");

            if (npc == null)
            {
                client.Send(new SystemMessagePacket($"NPC Shop not found."));
                client.Send(new NpcItemPurchasePacket(client.Tamer.Inventory.Bits));
                _logger.Error($"Unknown NPC id {npcId} at map {client.Tamer.Location.MapId}.");
                return;
            }

            var npcItem = npc.Items[shopSlot];

            var purchasingItem = new ItemModel();

            purchasingItem.SetItemId(npcItem.ItemId);
            purchasingItem.SetAmount(purchaseCount);
            purchasingItem.SetItemInfo(_assets.ItemInfo.First(x => x.ItemId == npcItem.ItemId));

            if (purchasingItem.IsTemporary)
                purchasingItem.SetRemainingTime((uint)purchasingItem.ItemInfo.UsageTimeMinutes);

            //_logger.Information($"Buying item {purchasingItem.ItemId} - {purchasingItem.ItemInfo.Name} | Amount: {purchaseCount}");

            if (purchasingItem.ItemInfo!.EventPriceId > 0)
            {
                var eventItemPrice = purchasingItem.ItemInfo.EventPriceAmount * purchaseCount;

                if (!client.Tamer.Inventory.RemoveOrReduceItemsByItemId(purchasingItem.ItemInfo.EventPriceId, eventItemPrice))
                {
                    client.Send(new SystemMessagePacket($"Insufficient required item amount."));
                    client.Send(new NpcItemPurchasePacket(client.Tamer.Inventory.Bits));
                    _logger.Warning($"Insufficient required item amount to buy item {npcItem.ItemId} from NPC {npcId} at {client.TamerLocation}.");
                    return;
                }

                _logger.Verbose($"Character {client.TamerId} purchased {purchasingItem.ItemId} x{purchasingItem.Amount} for " +
                    $"{purchasingItem.ItemInfo.EventPriceId} x{eventItemPrice} from NPC {npcId} at {client.TamerLocation}.");
            }
            else if (purchasingItem.ItemInfo.DigicorePrice > 0)
            {
                var digicorePrice = purchasingItem.ItemInfo.DigicorePrice * purchaseCount;
                if (!client.Tamer.Inventory.RemoveOrReduceItemsBySection(16100, digicorePrice))
                {
                    client.Send(new SystemMessagePacket($"Insufficient digicore amount."));
                    client.Send(new NpcItemPurchasePacket(client.Tamer.Inventory.Bits));
                    _logger.Warning($"Insufficient digicore amount to buy item {npcItem.ItemId} from NPC {npcId} at {client.TamerLocation}.");
                    return;
                }

                _logger.Verbose($"Character {client.TamerId} purchased {purchasingItem.ItemId} x{purchasingItem.Amount} for " +
                    $"{digicorePrice} digicore from NPC {npcId} at {client.TamerLocation}.");
            }
            else
            {
                var bitsPrice = purchasingItem.ItemInfo.ScanPrice * purchaseCount;

                if (!client.Tamer.Inventory.RemoveBits(bitsPrice))
                {
                    client.Send(new SystemMessagePacket($"Insufficient bits amount."));
                    client.Send(new NpcItemPurchasePacket(client.Tamer.Inventory.Bits));
                    _logger.Warning($"Insufficient bits amount to buy item {npcItem.ItemId} from NPC {npcId} at {client.TamerLocation}.");
                    return;
                }

                _logger.Verbose($"Character {client.TamerId} purchased {purchasingItem.ItemId} x{purchasingItem.Amount} for " +
                    $"{bitsPrice} bits from NPC {npcId} at {client.TamerLocation}.");
            }
            
            client.Tamer.Inventory.AddItem(purchasingItem);
            client.Send(new NpcItemPurchasePacket(client.Tamer.Inventory.Bits, purchasingItem));
            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));
        }
    }
}