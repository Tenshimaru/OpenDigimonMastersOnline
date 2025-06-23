using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Chat;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class MegaphoneMessagePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.MegaphoneMessage;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        
        public MegaphoneMessagePacketProcessor(MapServer mapServer, DungeonsServer dungeonsServer, EventServer eventServer, PvpServer pvpServer,
            ILogger logger, ISender sender)
        {
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var message = packet.ReadString();
            var unk = packet.ReadByte();
            var slot = packet.ReadByte();

            var inventoryItem = client.Tamer.Inventory.FindItemBySlot(slot);

            if (inventoryItem == null) 
            {
                client.Send(new SystemMessagePacket($"Unable to find item in slot {slot}."));
                _logger.Warning($"Item not found in slot {slot} for player {client.TamerId} megaphone.");
                return;
            }
            client.SendToAll(new ChatMessagePacket(message, ChatTypeEnum.Megaphone, client.Tamer.Name, 
                inventoryItem.ItemId, client.Tamer.Level).Serialize());
            /*_mapServer.BroadcastGlobal(new ChatMessagePacket(message, ChatTypeEnum.Megaphone, client.Tamer.Name, 
                inventoryItem.ItemId, client.Tamer.Level).Serialize());

            _dungeonServer.BroadcastGlobal(new ChatMessagePacket(message, ChatTypeEnum.Megaphone, client.Tamer.Name,
                inventoryItem.ItemId, client.Tamer.Level).Serialize());

            _eventServer.BroadcastGlobal(new ChatMessagePacket(message, ChatTypeEnum.Megaphone, client.Tamer.Name,
                inventoryItem.ItemId, client.Tamer.Level).Serialize());

            _pvpServer.BroadcastGlobal(new ChatMessagePacket(message, ChatTypeEnum.Megaphone, client.Tamer.Name,
                inventoryItem.ItemId, client.Tamer.Level).Serialize());*/

            await _mapServer.CallDiscord(message, client, "ffffff", "P");
            _logger.Verbose($"Character {client.TamerId} sent megaphone with item {inventoryItem.ItemId} and message {message}.");
            await _sender.Send(new CreateChatMessageCommand(ChatMessageModel.Create(client.TamerId, message)));

            if (!inventoryItem.ItemInfo.TemporaryItem)
            {
                client.Tamer.Inventory.RemoveOrReduceItem(inventoryItem, 1, slot);
                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            }

        }
    }
}