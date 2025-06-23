using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Items;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class EvolutionRideUnlockPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.EvolutionRideUnlock;

        private readonly ISender _sender;
        private readonly ILogger _logger;

        public EvolutionRideUnlockPacketProcessor(ISender sender, ILogger logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var evoIdx = packet.ReadInt() -1;
            var itemSection = packet.ReadInt();

            var inventoryItem = client.Tamer.Inventory.FindItemBySection(itemSection);

            client.Tamer.Inventory.RemoveOrReduceItem(inventoryItem, 1);

            client.Partner.Evolutions[evoIdx].UnlockRide();

            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

            _logger.Verbose($"Character {client.TamerId} unlocked {client.Partner.Evolutions[evoIdx].Type} " +
                $"ride mode for {client.Partner.Id} ({client.Partner.BaseType}) with item section {itemSection} x1.");

            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateEvolutionCommand(client.Partner.Evolutions[evoIdx]));
        }
    }
}