using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartyRequestResponsePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartyRequestResponse;

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly ILogger _logger;

        public PartyRequestResponsePacketProcessor(PartyManager partyManager, MapServer mapServer, DungeonsServer dungeonServer, ILogger logger)
        {
            _partyManager = partyManager;
            _dungeonServer = dungeonServer;
            _mapServer = mapServer;
            _logger = logger;
        }

        public Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var inviteResult = packet.ReadInt();
            var leaderName = packet.ReadString();

            var leaderClient = _mapServer.FindClientByTamerName(leaderName);

            if (leaderClient == null)
            {
                leaderClient = _dungeonServer.FindClientByTamerName(leaderName);

                if (leaderClient != null)
                {
                    var dungeonParty = _partyManager.FindParty(leaderClient.TamerId);

                    if (dungeonParty == null)
                    {
                        dungeonParty = _partyManager.CreateParty(leaderClient.Tamer, client.Tamer);
                        _logger.Verbose($"Character {leaderClient.TamerId} created party {dungeonParty.Id} with {client.TamerId}.");

                        if (leaderClient.DungeonMap)
                        {
                            var targetMap = _dungeonServer.Maps.FirstOrDefault(x => x.DungeonId == leaderClient.TamerId);

                            if (targetMap != null)
                            {
                                targetMap.SetId(dungeonParty.Id);
                            }
                        }

                        leaderClient.Send(new PartyCreatedPacket(dungeonParty.Id, dungeonParty.LootType));
                        leaderClient.Send(new PartyMemberJoinPacket(dungeonParty[client.TamerId], leaderClient.Tamer));
                        leaderClient.Send(new PartyMemberInfoPacket(dungeonParty[client.TamerId]));
                        leaderClient.Send(new PartyRequestSentFailedPacket(PartyRequestFailedResultEnum.Accept, client.Tamer.Name));

                        client.Send(new PartyMemberListPacket(dungeonParty, client.TamerId));

                    }
                    else
                    {
                        dungeonParty.AddMember(client.Tamer);
                        _logger.Verbose($"Character {client.TamerId} joinned party {dungeonParty.Id} (leader {leaderClient.TamerId}).");

                        client.Send(new PartyMemberListPacket(dungeonParty, client.TamerId, (byte)(dungeonParty.Members.Count - 1)));
                        leaderClient.Send(new PartyRequestSentFailedPacket(PartyRequestFailedResultEnum.Accept, client.Tamer.Name));

                        foreach (var target in dungeonParty.Members.Values)
                        {
                            var targetClient = _mapServer.FindClientByTamerId(target.Id);

                            if (targetClient == null) targetClient = _dungeonServer.FindClientByTamerId(target.Id);

                            if (targetClient == null) continue;

                            if (target.Id != client.Tamer.Id)
                            {
                                if (targetClient.Tamer.Id == dungeonParty.LeaderId)
                                {
                                    targetClient.Send(new PartyMemberJoinPacket(dungeonParty[client.TamerId], leaderClient.Tamer));
                                    targetClient.Send(new PartyMemberInfoPacket(dungeonParty[client.TamerId], true));
                                }
                                else
                                {
                                    targetClient.Send(new PartyMemberJoinPacket(dungeonParty[client.TamerId]));
                                    targetClient.Send(new PartyMemberInfoPacket(dungeonParty[client.TamerId]));
                                }
                            }
                        }
                    }

                    return Task.CompletedTask;
                }
                else
                {
                    _logger.Warning($"Unable to find party leader with name {leaderName}.");
                    client.Send(new SystemMessagePacket($"Unable to find party leader with name {leaderName}."));
                    return Task.CompletedTask;
                }
            }

            if (inviteResult == -1)
            {
                leaderClient.Send(new PartyRequestSentFailedPacket(PartyRequestFailedResultEnum.Rejected, client.Tamer.Name));
                _logger.Verbose($"Character {client.TamerId} refused party invite from {leaderClient.TamerId}.");
                return Task.CompletedTask;
            }

            var party = _partyManager.FindParty(leaderClient.TamerId);

            if (party == null)
            {
                party = _partyManager.CreateParty(leaderClient.Tamer, client.Tamer);
                _logger.Verbose($"Character {leaderClient.TamerId} created party {party.Id} with {client.TamerId}.");

                if (leaderClient.DungeonMap)
                {
                    var targetMap = _dungeonServer.Maps.FirstOrDefault(x => x.DungeonId == leaderClient.TamerId);

                    if (targetMap != null)
                    {
                        targetMap.SetId(party.Id);
                    }
                }

                /*leaderClient.Send(new PartyCreatedPacket(party.Id, party.LootType));
                leaderClient.Send(new PartyRequestSentFailedPacket(PartyRequestFailedResultEnum.Accept, client.Tamer.Name));
                leaderClient.Send(new PartyMemberJoinPacket(party[client.TamerId], leaderClient.Tamer));
                leaderClient.Send(new PartyMemberInfoPacket(party[client.TamerId]));
                leaderClient.Send(new PartyMemberMovimentationPacket(party[client.TamerId]));*/
                leaderClient.Send(
                    UtilitiesFunctions.GroupPackets(
                        new PartyCreatedPacket(party.Id, party.LootType).Serialize(),
                        new PartyRequestSentFailedPacket(PartyRequestFailedResultEnum.Accept, client.Tamer.Name).Serialize(),
                        new PartyMemberJoinPacket(party[client.TamerId], leaderClient.Tamer).Serialize(),
                        new PartyMemberInfoPacket(party[client.TamerId]).Serialize(),
                        new PartyMemberMovimentationPacket(party[client.TamerId]).Serialize()
                    ));
                
                client.Send(new PartyMemberListPacket(party, client.TamerId));

            }
            else
            {
                party.AddMember(client.Tamer);
                _logger.Verbose($"Character {client.TamerId} joinned party {party.Id} (leader {leaderClient.TamerId}).");

                client.Send(new PartyMemberListPacket(party, client.TamerId, (byte)(party.Members.Count - 1)));
                leaderClient.Send(new PartyRequestSentFailedPacket(PartyRequestFailedResultEnum.Accept, client.Tamer.Name));

                foreach (var target in party.Members.Values)
                {
                    var targetClient = _mapServer.FindClientByTamerId(target.Id);

                    if (targetClient == null) targetClient = _dungeonServer.FindClientByTamerId(target.Id);

                    if (targetClient == null) continue;

                    if (target.Id != client.Tamer.Id)
                    {
                        targetClient.Send(new PartyMemberJoinPacket(party[client.TamerId], targetClient.Tamer));
                        targetClient.Send(new PartyMemberInfoPacket(party[client.TamerId]));
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}