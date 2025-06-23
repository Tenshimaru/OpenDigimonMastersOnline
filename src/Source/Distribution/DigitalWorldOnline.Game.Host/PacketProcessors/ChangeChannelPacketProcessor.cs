using AutoMapper;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ChangeChannelSendProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ChangeChannel;

        private const string GameServerAddress = "GameServer:Address";
        private const string GamerServerPublic = "GameServer:PublicAddress";
        private const string GameServerPort = "GameServer:Port";

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ChangeChannelSendProcessor(PartyManager partyManager, MapServer mapServer, EventServer eventServer,
            PvpServer pvpServer,
            ILogger logger, ISender sender, IMapper mapper, IConfiguration configuration)
        {
            _partyManager = partyManager;
            _mapServer = mapServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            byte NewChannel = packet.ReadByte();

            var oldChannel = client.Tamer.Channel;

            client.Tamer.SetCurrentChannel(NewChannel);
            await _sender.Send(new UpdateCharacterChannelCommand(client.TamerId, NewChannel));

            _logger.Debug($"Tamer {client.TamerId}:{client.Tamer.Name} change Channel {oldChannel} to {NewChannel}");

            // -- RELOAD -----------------------------------------------------------------

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            client.Tamer.UpdateState(CharacterStateEnum.Loading);

            await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

            switch (mapConfig.Type)
            {
                case MapTypeEnum.Event:
                    _eventServer.RemoveClient(client);
                    break;
                case MapTypeEnum.Pvp:
                    _pvpServer.RemoveClient(client);
                    break;
                default:
                    _mapServer.RemoveClient(client);
                    break;
            }

            client.SetGameQuit(false);

            client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y));

            // -- PARTY VERIFICATION -----------------------------------------------------

            var party = _partyManager.FindParty(client.TamerId);

            if (party != null)
            {
                party.UpdateMember(party[client.TamerId], client.Tamer);

                foreach (var target in party.Members.Values)
                {
                    if (target.Id != client.Tamer.Id)
                    {
                        _mapServer.BroadcastForTamerViewsAndSelf(client,
                            new PartyMemberWarpGatePacket(party[client.TamerId], client.Tamer).Serialize());
                        _eventServer.BroadcastForTamerViewsAndSelf(client,
                            new PartyMemberWarpGatePacket(party[client.TamerId], client.Tamer).Serialize());
                    }
                }
            }
        }
    }
}