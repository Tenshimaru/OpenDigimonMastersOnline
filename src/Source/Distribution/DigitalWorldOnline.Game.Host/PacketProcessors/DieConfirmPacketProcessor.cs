using AutoMapper;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class DieConfirmPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.DieConfirm;

        private const string GameServerAddress = "GameServer:Address";
        private const string GamerServerPublic = "GameServer:PublicAddress";
        private const string GameServerPort = "GameServer:Port";

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        
        public DieConfirmPacketProcessor(PartyManager partyManager, MapServer mapServer, EventServer eventServer, DungeonsServer dungeonsServer, PvpServer pvpServer,
            ISender sender, IMapper mapper, IConfiguration configuration)
        {
            _partyManager = partyManager;
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _sender = sender;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var mapTypeConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            switch (mapTypeConfig!.Type)
            {
                case MapTypeEnum.Dungeon:
                    {
                        client.Tamer.Die();

                        var party = _partyManager.FindParty(client.TamerId);

                        var map = UtilitiesFunctions.MapGroup(client.Tamer.Location.MapId);

                        bool shouldReviveInSameMap = false;

                        if (party != null && party.Members.Values.Count(x =>
                                x.Id != client.TamerId && x.Location.MapId == client.Tamer.Location.MapId && x.Channel == client.Tamer.Channel) > 0)
                        {
                            map = client.Tamer.Location.MapId;
                            shouldReviveInSameMap = true;
                        }

                        var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(map));
                        var waypoints = await _sender.Send(new MapRegionListAssetsByMapIdQuery(map));

                        if (mapConfig == null || waypoints == null || !waypoints.Regions.Any())
                        {
                            client.Send(new SystemMessagePacket($"Map information not found for map Id {map}."));
                            return;
                        }

                        var destination = waypoints.Regions.First();

                        await _sender.Send(new UpdateCharacterBasicInfoCommand(client.Tamer));
                        await _sender.Send(new UpdateCharacterActiveEvolutionCommand(client.Tamer.ActiveEvolution));

                        client.Tamer.NewLocation(map, destination.X, destination.Y);
                        await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));

                        client.Tamer.Partner.NewLocation(map, destination.X, destination.Y);
                        await _sender.Send(new UpdateDigimonLocationCommand(client.Tamer.Partner.Location));

                        // -- RELOAD -----------------------------------------------------------------

                        client.Tamer.UpdateState(CharacterStateEnum.Loading);
                        await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

                        if (shouldReviveInSameMap)
                        {
                            _dungeonServer.RemoveClient(client);

                            client.SetGameQuit(false);
                        }
                        else
                        {
                            _dungeonServer.RemoveClient(client);
                        }

                        client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                                client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y).Serialize());
                    }
                    break;

                case MapTypeEnum.Event:
                    {
                        var destiny = _mapper.Map<MapRegionListAssetModel>(await _sender.Send(new MapRegionListAssetsByMapIdQuery(client.Tamer.Location.MapId)));
                        var region = destiny?.Regions.FirstOrDefault();

                        if (region != null)
                        {
                            client.Tamer.NewLocation(region.X, region.Y);
                            await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));

                            client.Tamer.Partner.NewLocation(region.X, region.Y);
                            await _sender.Send(new UpdateDigimonLocationCommand(client.Tamer.Partner.Location));
                        }

                        client.Tamer.UpdateState(CharacterStateEnum.Loading);
                        await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

                        _eventServer.RemoveClient(client);

                        client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                                client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y).Serialize());
                    }
                    break;

                case MapTypeEnum.Pvp:
                    {
                        var destiny = _mapper.Map<MapRegionListAssetModel>(await _sender.Send(new MapRegionListAssetsByMapIdQuery(client.Tamer.Location.MapId)));
                        var region = destiny?.Regions.FirstOrDefault();

                        if (region != null)
                        {
                            client.Tamer.NewLocation(region.X, region.Y);
                            await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));

                            client.Tamer.Partner.NewLocation(region.X, region.Y);
                            await _sender.Send(new UpdateDigimonLocationCommand(client.Tamer.Partner.Location));
                        }

                        client.Tamer.UpdateState(CharacterStateEnum.Loading);
                        await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

                        _pvpServer.RemoveClient(client);

                        client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                                client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y).Serialize());
                    }
                    break;

                default:
                    {
                        var destiny = _mapper.Map<MapRegionListAssetModel>(await _sender.Send(new MapRegionListAssetsByMapIdQuery(client.Tamer.Location.MapId)));
                        var region = destiny?.Regions.FirstOrDefault();

                        if (region != null)
                        {
                            client.Tamer.NewLocation(region.X, region.Y);
                            await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));

                            client.Tamer.Partner.NewLocation(region.X, region.Y);
                            await _sender.Send(new UpdateDigimonLocationCommand(client.Tamer.Partner.Location));
                        }

                        client.Tamer.UpdateState(CharacterStateEnum.Loading);
                        await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

                        _mapServer.RemoveClient(client);

                        client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                                client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y).Serialize());
                    }
                    break;
            }

        }
    }
}