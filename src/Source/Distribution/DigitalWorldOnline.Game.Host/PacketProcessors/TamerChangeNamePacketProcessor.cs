using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.GameHost.EventsServer;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Packets.Chat;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class TamerChangeNamePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TamerChangeName;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public TamerChangeNamePacketProcessor(
            MapServer mapServer,
            DungeonsServer dungeonServer,
            EventServer eventServer,
            PvpServer pvpServer,
            ILogger logger,
            ISender sender)
        {
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            int itemSlot = packet.ReadInt();
            var newName = packet.ReadString();

            var oldName = client.Tamer.Name;
            var AvaliabeName = await _sender.Send(new CharacterByNameQuery(newName)) == null;

            if (!AvaliabeName)
            {
                client.Send(new TamerChangeNamePacket(CharacterChangeNameType.Existing, oldName, newName, itemSlot));
                return;
            }

            var inventoryItem = client.Tamer.Inventory.FindItemBySlot(itemSlot);

            if (inventoryItem != null)
            {
                if (inventoryItem?.ItemInfo.Section != 15200)
                {
                    _logger.Error($"The Player {client.Tamer.Name} tryed to change tamer name with the incorrect item: {inventoryItem.ItemId} - {inventoryItem.ItemInfo.Name}");

                    var banProcessor = SingletonResolver.GetService<BanForCheating>();
                    var banMessage = banProcessor.BanAccountWithMessage(client.AccountId, client.Tamer.Name, AccountBlockEnum.Permanent, "Cheating", client, "You tried to change your tamer name using a cheat method, So be happy with ban!");

                    client.SendToAll(new NoticeMessagePacket(banMessage).Serialize());
                    // client.Send(new DisconnectUserPacket($"GAME DISCONNECTED: TRYING TO USE CHEAT").Serialize());

                    return;
                }

                client.Tamer.Inventory.RemoveOrReduceItem(inventoryItem, 1, itemSlot);
                client.Tamer.UpdateName(newName);

                await _sender.Send(new ChangeTamerNameByIdCommand(client.Tamer.Id, newName));
                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));

                client.Send(new TamerChangeNamePacket(CharacterChangeNameType.Sucess, itemSlot, oldName, newName));
                client.Send(new TamerChangeNamePacket(CharacterChangeNameType.Complete, newName, newName, itemSlot));
                var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));
                
                switch (mapConfig?.Type)
                {
                    case MapTypeEnum.Dungeon:
                        _dungeonServer.BroadcastForTamerViews(client.TamerId, UtilitiesFunctions.GroupPackets(
                            new UnloadTamerPacket(client.Tamer).Serialize(),
                            new LoadTamerPacket(client.Tamer).Serialize()
                        ));
                        break;

                    case MapTypeEnum.Event:
                        _eventServer.BroadcastForTamerViews(client, UtilitiesFunctions.GroupPackets(
                            new UnloadTamerPacket(client.Tamer).Serialize(),
                            new LoadTamerPacket(client.Tamer).Serialize()
                        ));
                        break;

                    case MapTypeEnum.Pvp:
                        _pvpServer.BroadcastForTamerViews(client, UtilitiesFunctions.GroupPackets(
                            new UnloadTamerPacket(client.Tamer).Serialize(),
                            new LoadTamerPacket(client.Tamer).Serialize()
                        ));
                        break;

                    default:
                        _mapServer.BroadcastForTamerViews(client, UtilitiesFunctions.GroupPackets(
                            new UnloadTamerPacket(client.Tamer).Serialize(),
                            new LoadTamerPacket(client.Tamer).Serialize()
                        ));
                        break;
                }


                List<long> friendsIds = client.Tamer.Friended.Select(x => x.CharacterId).ToList();

                _mapServer.BroadcastForTargetTamers(friendsIds,
                    new FriendChangeNamePacket(oldName, newName, false).Serialize());
                _dungeonServer.BroadcastForTargetTamers(friendsIds,
                    new FriendChangeNamePacket(oldName, newName, false).Serialize());
                _eventServer.BroadcastForTargetTamers(friendsIds,
                    new FriendChangeNamePacket(oldName, newName, false).Serialize());
                _pvpServer.BroadcastForTargetTamers(friendsIds,
                    new FriendChangeNamePacket(oldName, newName, false).Serialize());

                List<long> foesIds = client.Tamer.Foed.Select(x => x.CharacterId).ToList();

                _mapServer.BroadcastForTargetTamers(foesIds,
                    new FriendChangeNamePacket(oldName, newName, true).Serialize());
                _dungeonServer.BroadcastForTargetTamers(foesIds,
                    new FriendChangeNamePacket(oldName, newName, true).Serialize());
                _eventServer.BroadcastForTargetTamers(foesIds,
                    new FriendChangeNamePacket(oldName, newName, true).Serialize());
                _pvpServer.BroadcastForTargetTamers(foesIds,
                    new FriendChangeNamePacket(oldName, newName, true).Serialize());

                _logger.Verbose($"Character {client.TamerId} Changed Name {oldName} to {newName}.");
            }
        }
    }
}