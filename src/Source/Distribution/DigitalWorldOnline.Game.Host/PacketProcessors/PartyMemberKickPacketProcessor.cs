using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartyMemberKickPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartyMemberKick;

        private const string GameServerAddress = "GameServer:Address";
        private const string GamerServerPublic = "GameServer:PublicAddress";
        private const string GameServerPort = "GameServer:Port";

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IConfiguration _configuration;

        public PartyMemberKickPacketProcessor(
            PartyManager partyManager,
            MapServer mapServer,
            DungeonsServer dungeonServer,
            EventServer eventServer,
            PvpServer pvpServer,
            ILogger logger, 
            ISender sender,
            IConfiguration configuration
            )
        {
            _partyManager = partyManager;
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
            _configuration = configuration;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var targetName = packet.ReadString();

            var party = _partyManager.FindParty(client.TamerId);

            if (party != null)
            {
                var membersList = party.GetMembersIdList();
                var bannedTargetKey = party[targetName].Key;
                var partyMemberToKick = party.Members.First(x => x.Value.Name == targetName);
               
                if (party.Members.Count > 2)
                {
                    //_logger.Information($"{party.Members.Count} players was on party !!");

                    var partyMember = party[targetName].Value;

                    var bannedClient = ((_mapServer.FindClientByTamerId(partyMember.Id) ?? _dungeonServer.FindClientByTamerId(partyMember.Id)) ??
                                        _eventServer.FindClientByTamerId(partyMember.Id)) ?? _pvpServer.FindClientByTamerId(partyMember.Id);

                    bannedClient?.Send(new PartyMemberKickPacket(bannedTargetKey).Serialize());

                    party.RemoveMember(bannedTargetKey);

                    // -------------------------------------------------------

                    var dungeonClient = _dungeonServer.FindClientByTamerId(partyMember.Id);

                    if (dungeonClient != null)
                    {
                        //_logger.Information($"Kicked Player is in Dungeon");

                        var map = UtilitiesFunctions.MapGroup(dungeonClient.Tamer.Location.MapId);

                        var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(dungeonClient.Tamer.Location.MapId));
                        var waypoints = await _sender.Send(new MapRegionListAssetsByMapIdQuery(map));

                        if (mapConfig == null || waypoints == null || !waypoints.Regions.Any())
                        {
                            client.Send(new SystemMessagePacket($"Map information not found for map Id {map}."));
                            _logger.Warning($"Map information not found for map Id {map} on character {client.TamerId} jump booster.");
                            return;
                        }

                        var mapRegionIndex = mapConfig.MapRegionindex;
                        var destination = waypoints.Regions.FirstOrDefault(x => x.Index == mapRegionIndex);

                        _dungeonServer.RemoveClient(dungeonClient);

                        dungeonClient.Tamer.NewLocation(map, destination.X, destination.Y);
                        await _sender.Send(new UpdateCharacterLocationCommand(dungeonClient.Tamer.Location));

                        dungeonClient.Tamer.Partner.NewLocation(map, destination.X, destination.Y);
                        await _sender.Send(new UpdateDigimonLocationCommand(dungeonClient.Tamer.Partner.Location));

                        dungeonClient.Tamer.UpdateState(CharacterStateEnum.Loading);
                        await _sender.Send(new UpdateCharacterStateCommand(dungeonClient.TamerId, CharacterStateEnum.Loading));

                        dungeonClient.SetGameQuit(false);

                        dungeonClient.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                            dungeonClient.Tamer.Location.MapId, dungeonClient.Tamer.Location.X, dungeonClient.Tamer.Location.Y));
                    }

                    foreach (var targetClient in party.Members.Values.Select(target => ((_mapServer.FindClientByTamerId(target.Id) ?? _dungeonServer.FindClientByTamerId(target.Id)) ??
                                 _eventServer.FindClientByTamerId(target.Id)) ?? _pvpServer.FindClientByTamerId(target.Id)))
                    {
                        targetClient?.Send(new PartyMemberKickPacket(bannedTargetKey).Serialize());
                    }

                }
                else
                {
                    //_logger.Information($"No more players on party !! Removing Party");

                    _dungeonServer.BroadcastForTargetTamers(membersList, new PartyMemberKickPacket(partyMemberToKick.Key).Serialize());
                    _mapServer.BroadcastForTargetTamers(membersList, new PartyMemberKickPacket(partyMemberToKick.Key).Serialize());
                    _eventServer.BroadcastForTargetTamers(membersList, new PartyMemberKickPacket(partyMemberToKick.Key).Serialize());
                    _pvpServer.BroadcastForTargetTamers(membersList, new PartyMemberKickPacket(partyMemberToKick.Key).Serialize());
                    
                    var memberList = party.Members.Values;

                    foreach (var target in memberList)
                    {
                        var targetClient = _dungeonServer.FindClientByTamerId(target.Id);

                        if (targetClient == null) continue;

                        // -- Teleport player outside of Dungeon ---------------------------------
                        var map = UtilitiesFunctions.MapGroup(targetClient.Tamer.Location.MapId);

                        var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(targetClient.Tamer.Location.MapId));
                        var waypoints = await _sender.Send(new MapRegionListAssetsByMapIdQuery(map));

                        if (mapConfig == null || waypoints == null || !waypoints.Regions.Any())
                        {
                            client.Send(new SystemMessagePacket($"Map information not found for map Id {map}."));
                            _logger.Warning($"Map information not found for map Id {map} on character {client.TamerId} jump booster.");
                            return;
                        }

                        var mapRegionIndex = mapConfig.MapRegionindex;
                        var destination = waypoints.Regions.FirstOrDefault(x => x.Index == mapRegionIndex);

                        _dungeonServer.RemoveClient(targetClient);

                        targetClient.Tamer.NewLocation(map, destination.X, destination.Y);
                        await _sender.Send(new UpdateCharacterLocationCommand(targetClient.Tamer.Location));

                        targetClient.Tamer.Partner.NewLocation(map, destination.X, destination.Y);
                        await _sender.Send(new UpdateDigimonLocationCommand(targetClient.Tamer.Partner.Location));

                        targetClient.Tamer.UpdateState(CharacterStateEnum.Loading);
                        await _sender.Send(new UpdateCharacterStateCommand(targetClient.TamerId, CharacterStateEnum.Loading));

                        targetClient.SetGameQuit(false);

                        targetClient.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                            targetClient.Tamer.Location.MapId, targetClient.Tamer.Location.X, targetClient.Tamer.Location.Y));
                    }

                    _partyManager.RemoveParty(party.Id);
                }
            }
            else
            {
                client.Send(new SystemMessagePacket($"The target tamer is not in a party."));
                _logger.Warning($"Character {client.TamerId} kicked {targetName} from the party but he/she was not in the party.");
                return;
            }
        }
    }
}