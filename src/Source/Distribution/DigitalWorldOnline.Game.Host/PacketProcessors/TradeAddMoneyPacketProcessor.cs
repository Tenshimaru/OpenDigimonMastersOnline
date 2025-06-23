using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class TradeAddMoneyacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TradeAddMoney;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public TradeAddMoneyacketProcessor(MapServer mapServer, DungeonsServer dungeonsServer, EventServer eventServer, PvpServer pvpServer, ILogger logger, ISender sender)
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

            var TargetMoney = packet.ReadInt();

            GameClient? targetClient;

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            switch (mapConfig!.Type)
            {
                case MapTypeEnum.Dungeon:
                    targetClient = _dungeonServer.FindClientByTamerHandleAndChannel(client.Tamer.TargetTradeGeneralHandle, client.TamerId);
                    break;

                case MapTypeEnum.Event:
                    targetClient = _eventServer.FindClientByTamerHandleAndChannel(client.Tamer.TargetTradeGeneralHandle, client.TamerId);
                    break;

                case MapTypeEnum.Pvp:
                    targetClient = _pvpServer.FindClientByTamerHandleAndChannel(client.Tamer.TargetTradeGeneralHandle, client.TamerId);
                    break;

                default:
                    targetClient = _mapServer.FindClientByTamerHandleAndChannel(client.Tamer.TargetTradeGeneralHandle, client.TamerId);
                    break;
            }

            if (client.Tamer.Inventory.Bits < TargetMoney)
            {
                //sistema de banimento permanente
                targetClient?.Tamer.ClearTrade();
                targetClient?.Send(new TradeInventoryUnlockPacket(client.Tamer.TargetTradeGeneralHandle));
                targetClient?.Send(new TradeCancelPacket(client.Tamer.GeneralHandler));
                
                var banProcessor = SingletonResolver.GetService<BanForCheating>();
                var banMessage = banProcessor.BanAccountWithMessage(client.AccountId, client.Tamer.Name, AccountBlockEnum.Permanent, "Cheating", client, "You tried to trade invalid amount of bits using a cheat method, So be happy with ban!");

                var chatPacket = new NoticeMessagePacket(banMessage).Serialize();
                client.SendToAll(chatPacket);
                return;
            }
            
            client.Tamer.TradeInventory.AddBits(TargetMoney);

            targetClient?.Send(new TradeInventoryUnlockPacket(client.Tamer.TargetTradeGeneralHandle));
            client.Send(new TradeInventoryUnlockPacket(client.Tamer.TargetTradeGeneralHandle));

            client.Send(new TradeAddMoneyPacket(client.Tamer.GeneralHandler, TargetMoney));
            targetClient?.Send(new TradeAddMoneyPacket(client.Tamer.GeneralHandler, TargetMoney));

            //_logger.Verbose($"Character {client.TamerId} and {targetClient.TamerId} inventory unlock "); ;
        }

    }
}
