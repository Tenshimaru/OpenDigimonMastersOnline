using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class MovimentationPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TamerMovimentation;

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ISender _sender;

        public MovimentationPacketProcessor(PartyManager partyManager, MapServer mapServer, DungeonsServer dungeonServer,
            EventServer eventServer, PvpServer pvpServer, ISender sender)
        {
            _partyManager = partyManager;
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var ticks = packet.ReadUInt();
            var handler = packet.ReadUInt();
            var newX = packet.ReadInt();
            var newY = packet.ReadInt();
            var newZ = packet.ReadFloat();

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            if (client.Tamer.PreviousCondition == ConditionEnum.Ride && client.Tamer.CurrentCondition == ConditionEnum.Away)
            {
                client.Tamer.ResetAfkNotifications();
                client.Tamer.UpdateCurrentCondition(ConditionEnum.Ride);

                switch (mapConfig?.Type)
                {
                    case MapTypeEnum.Dungeon:
                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId, new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition).Serialize());
                        break;

                    case MapTypeEnum.Event:
                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition).Serialize());
                        break;

                    case MapTypeEnum.Pvp:
                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId, new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition).Serialize());
                        break;

                    default:
                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition).Serialize());
                        break;
                }

            }

            if (client.Tamer.Riding)
            {
                client.Tamer.NewLocation(newX, newY, newZ);
                client.Tamer.Partner.NewLocation(newX, newY, newZ);

                switch (mapConfig?.Type)
                {
                    case MapTypeEnum.Dungeon:
                        {
                            _dungeonServer.BroadcastForTargetTamers(client.TamerId, new TamerWalkPacket(client.Tamer).Serialize());
                            _dungeonServer.BroadcastForTargetTamers(client.TamerId, new DigimonWalkPacket(client.Tamer.Partner).Serialize());
                        }
                        break;

                    case MapTypeEnum.Event:
                        {
                            _eventServer.BroadcastForTargetTamers(client.TamerId, new TamerWalkPacket(client.Tamer).Serialize());
                            _eventServer.BroadcastForTargetTamers(client.TamerId, new DigimonWalkPacket(client.Tamer.Partner).Serialize());
                        }
                        break;

                    case MapTypeEnum.Pvp:
                        {
                            _pvpServer.BroadcastForTargetTamers(client.TamerId, new TamerWalkPacket(client.Tamer).Serialize());
                            _pvpServer.BroadcastForTargetTamers(client.TamerId, new DigimonWalkPacket(client.Tamer.Partner).Serialize());
                        }
                        break;

                    default:
                        {
                            _mapServer.BroadcastForTargetTamers(client.TamerId, new TamerWalkPacket(client.Tamer).Serialize());
                            _mapServer.BroadcastForTargetTamers(client.TamerId, new DigimonWalkPacket(client.Tamer.Partner).Serialize());
                        }
                        break;
                }

            }
            else
            {
                if (client.Tamer.CurrentCondition == ConditionEnum.Away)
                {
                    client.Tamer.ResetAfkNotifications();
                    client.Tamer.UpdateCurrentCondition(ConditionEnum.Default);

                    switch (mapConfig?.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonServer.BroadcastForTargetTamers(client.TamerId, new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition).Serialize());
                            break;

                        case MapTypeEnum.Event:
                            _eventServer.BroadcastForTargetTamers(client.TamerId, new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition).Serialize());
                            break;

                        case MapTypeEnum.Pvp:
                            _pvpServer.BroadcastForTargetTamers(client.TamerId, new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition).Serialize());
                            break;

                        default:
                            _mapServer.BroadcastForTargetTamers(client.TamerId, new SyncConditionPacket(client.Tamer.GeneralHandler, client.Tamer.CurrentCondition).Serialize());
                            break;
                    }

                }

                if (handler >= short.MaxValue)
                {
                    client.Tamer.NewLocation(newX, newY, newZ);

                    switch (mapConfig?.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonServer.BroadcastForTargetTamers(client.TamerId, new TamerWalkPacket(client.Tamer).Serialize());
                            break;

                        case MapTypeEnum.Event:
                            _eventServer.BroadcastForTargetTamers(client.TamerId, new TamerWalkPacket(client.Tamer).Serialize());
                            break;

                        case MapTypeEnum.Pvp:
                            _pvpServer.BroadcastForTargetTamers(client.TamerId, new TamerWalkPacket(client.Tamer).Serialize());
                            break;

                        default:
                            _mapServer.BroadcastForTargetTamers(client.TamerId, new TamerWalkPacket(client.Tamer).Serialize());
                            break;
                    }

                }
                else
                {
                    client.Tamer.Partner.NewLocation(newX, newY, newZ);

                    switch (mapConfig?.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonServer.BroadcastForTargetTamers(client.TamerId, new DigimonWalkPacket(client.Tamer.Partner).Serialize());
                            break;

                        case MapTypeEnum.Event:
                            _eventServer.BroadcastForTargetTamers(client.TamerId, new DigimonWalkPacket(client.Tamer.Partner).Serialize());
                            break;

                        case MapTypeEnum.Pvp:
                            _pvpServer.BroadcastForTargetTamers(client.TamerId, new DigimonWalkPacket(client.Tamer.Partner).Serialize());
                            break;

                        default:
                            _mapServer.BroadcastForTargetTamers(client.TamerId, new DigimonWalkPacket(client.Tamer.Partner).Serialize());
                            break;
                    }

                }
            }

            var party = _partyManager.FindParty(client.TamerId);

            if (party != null)
            {
                party.UpdateMember(party[client.TamerId], client.Tamer);

                party.Members.Values.Where(x => x.Id != client.TamerId).ToList().ForEach(member =>
                    {
                        _mapServer.BroadcastForUniqueTamer(member.Id, new PartyMemberMovimentationPacket(party[client.TamerId]).Serialize());
                        _dungeonServer.BroadcastForUniqueTamer(member.Id, new PartyMemberMovimentationPacket(party[client.TamerId]).Serialize());
                        _eventServer.BroadcastForUniqueTamer(member.Id, new PartyMemberMovimentationPacket(party[client.TamerId]).Serialize());
                        _pvpServer.BroadcastForUniqueTamer(member.Id, new PartyMemberMovimentationPacket(party[client.TamerId]).Serialize());
                    });
            }

            await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));
            await _sender.Send(new UpdateDigimonLocationCommand(client.Partner.Location));

        }
    }
}
