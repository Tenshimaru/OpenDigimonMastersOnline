using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Mechanics;
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
    public class PartyMemberLeavePacketProcessor :IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartyMemberLeave;

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IConfiguration _configuration;

        public PartyMemberLeavePacketProcessor(
            PartyManager partyManager,
            MapServer mapServer,
            DungeonsServer dungeonServer,
            EventServer eventServer,
            PvpServer pvpServer,
            ILogger logger,
            ISender sender,
            IConfiguration configuration)
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

        public async Task Process(GameClient client,byte[] packetData)
        {
            _logger.Information($"Party Leave Packet received from {client.Tamer.Name}!");

            var party = _partyManager.FindParty(client.TamerId);
            if (party == null)
            {
                _logger.Error($"Tamer {client.Tamer.Name} attempted to leave a party but was not in one.");
                return;
            }

            var memberEntry = party.GetMemberById(client.TamerId);
            if (memberEntry == null)
            {
                _logger.Warning($"Tamer {client.Tamer.Name} was not found in party members.");
                return;
            }

            try
            {
                var leaveTargetKey = memberEntry.Value.Key;
                var leaveTargetId = memberEntry.Value.Value.Id;
                _logger.Information($"Processing leave request for {client.Tamer.Name} (Key: {leaveTargetKey}, ID: {leaveTargetId})");

                if (party.LeaderId == leaveTargetId && party.Members.Count > 2)
                {
                    _logger.Information($"{client.Tamer.Name} was the leader. Assigning new leader...");

                    NotifyPartyMembers(party,leaveTargetKey);
                    party.RemoveMember(leaveTargetKey);

                    var remainingMembers = party.GetMembersIdList().Where(id => id != leaveTargetId).ToList();
                    if (remainingMembers.Count > 0)
                    {
                        var newLeaderId = remainingMembers[new Random().Next(remainingMembers.Count)];
                        var newLeaderEntry = party.GetMemberById(newLeaderId);
                        var leaderSlot = newLeaderEntry.Value.Key;

                        if (newLeaderEntry != null)
                        {
                            party.ChangeLeader(newLeaderEntry.Value.Key);
                            BroadcastToAllServers(party.GetMembersIdList(),new PartyLeaderChangedPacket((int)leaderSlot).Serialize());
                        }
                    }
                }
                else if (party.Members.Count <= 2)
                {
                    _logger.Information($"{client.Tamer.Name} left the party. Remaining members: {party.Members.Count}");

                    NotifyPartyMembers(party,leaveTargetKey);

                    if (!client.DungeonMap)
                    {
                        party.RemoveMember(leaveTargetKey);
                    }
                    else
                    {
                        await HandleDungeonExit(client,party,leaveTargetKey);
                    }

                    _partyManager.RemoveParty(party.Id);
                }
                else
                {
                    NotifyPartyMembers(party,leaveTargetKey);
                    party.RemoveMember(leaveTargetKey);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error processing Party Leave Packet: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void NotifyPartyMembers(GameParty party,int leaveTargetKey)
        {
            _logger.Information($"Notifying party members that {leaveTargetKey} has left.");

            foreach (var target in party.Members.Values)
            {
                var targetClient = (_mapServer.FindClientByTamerId(target.Id) ??
                                    _dungeonServer.FindClientByTamerId(target.Id) ??
                                    _eventServer.FindClientByTamerId(target.Id) ??
                                    _pvpServer.FindClientByTamerId(target.Id));

                if (targetClient == null)
                {
                    _logger.Warning($"Client for TamerId {target.Id} not found. Skipping notification.");
                    continue;
                }

                _logger.Information($"Sending PartyMemberLeavePacket to {target.Id}");
                targetClient.Send(new PartyMemberLeavePacket((byte)leaveTargetKey).Serialize());

            }
        }

        private void BroadcastToAllServers(List<long> membersList,byte[] packetData)
        {
            _mapServer.BroadcastForTargetTamers(membersList,packetData);
            _dungeonServer.BroadcastForTargetTamers(membersList,packetData);
            _eventServer.BroadcastForTargetTamers(membersList,packetData);
            _pvpServer.BroadcastForTargetTamers(membersList,packetData);
        }

        private async Task HandleDungeonExit(GameClient client,GameParty? party,int leaveTargetKey)
        {
            var map = UtilitiesFunctions.MapGroup(client.Tamer.Location.MapId);
            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));
            var waypoints = await _sender.Send(new MapRegionListAssetsByMapIdQuery(map));

            if (mapConfig == null || waypoints == null || !waypoints.Regions.Any())
            {
                client.Send(new SystemMessagePacket($"Map information not found for map Id {map}."));
                _logger.Warning($"Map info missing for map Id {map} on character {client.TamerId}.");
                return;
            }

            var mapRegionIndex = mapConfig.MapRegionindex;
            var destination = waypoints.Regions.FirstOrDefault(x => x.Index == mapRegionIndex);

            _dungeonServer.RemoveClient(client);

            client.Tamer.NewLocation(map,destination.X,destination.Y);
            await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));

            client.Tamer.Partner.NewLocation(map,destination.X,destination.Y);
            await _sender.Send(new UpdateDigimonLocationCommand(client.Tamer.Partner.Location));

            client.Tamer.UpdateState(CharacterStateEnum.Loading);
            await _sender.Send(new UpdateCharacterStateCommand(client.TamerId,CharacterStateEnum.Loading));

            client.SetGameQuit(false);

            client.Send(new MapSwapPacket(_configuration["GameServer:PublicAddress"],
                                          _configuration["GameServer:Port"],
                                          client.Tamer.Location.MapId,
                                          client.Tamer.Location.X,
                                          client.Tamer.Location.Y));

            party.RemoveMember((byte)leaveTargetKey);
            _dungeonServer.BroadcastForTargetTamers(party.GetMembersIdList(),new PartyMemberLeavePacket((byte)leaveTargetKey).Serialize());
        }
    }
}
