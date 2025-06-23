using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.Items;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class NpcSellPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.NpcItemSell;

        private readonly ISender _sender;
        private readonly ILogger _logger;

        public NpcSellPacketProcessor(
            ISender sender,
            ILogger logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);
            var npcId = packet.ReadInt();
            var unk = packet.ReadByte();
            var itemSlot = packet.ReadByte();
            var sellAmount = packet.ReadShort();

            var sellItem = client.Tamer.Inventory.FindItemBySlot(itemSlot);
            if (sellItem == null)
            {
                client.Send(new SystemMessagePacket($"Item not found."));
                client.Send(new NpcItemSellPacket(client.Tamer.Inventory.Bits));
                _logger.Error($"Unknown item at slot {itemSlot}.");
                return;
            }

            if (sellItem.Amount < sellAmount)
            {
                _logger.Error($"[DISCONNECTED] {client.Tamer.Name} try SELL {sellAmount}x {sellItem.ItemInfo.Name}, but he has {sellItem.Amount}x!");
                client.Disconnect();
                return;
            }

            var bits = sellAmount * sellItem.ItemInfo!.SellPrice;
            client.Tamer.Inventory.AddBits(bits);

            _logger.Verbose($"Character {client.TamerId} sold {sellItem.ItemId} x{sellAmount} for {bits} bits on NPC {npcId} at {client.TamerLocation}.");

            client.Send(new NpcItemSellPacket(client.Tamer.Inventory.Bits, sellItem));
            //client.Tamer.AddRepurchaseItem((ItemModel)sellItem.Clone());
            //client.Send(new NpcRepurchaseListPacket(client.Tamer.RepurchaseList));
            client.Tamer.Inventory.RemoveOrReduceItem(sellItem, sellAmount, itemSlot);

            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

            await _sender.Send(new UpdateItemCommand(sellItem));
            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));
        }
    }
}