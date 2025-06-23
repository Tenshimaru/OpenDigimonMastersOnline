using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartyLeaderChangePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartyLeaderChange;

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;

        public PartyLeaderChangePacketProcessor(
            PartyManager partyManager,
            MapServer mapServer,
            DungeonsServer dungeonServer,
            EventServer eventServer,
            PvpServer pvpServer,
            ILogger logger)
        {
            _partyManager = partyManager;
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
        }

        public Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var newLeaderSlot = packet.ReadInt();

            var party = _partyManager.FindParty(client.TamerId);

            if (party != null)
            {
                party.ChangeLeader((byte)newLeaderSlot);

                //foreach (var memberId in party.GetMembersIdList())
                //{
                    //var targetMessage = _mapServer.FindClientByTamerId(memberId);
                    //if (targetMessage == null) targetMessage = _dungeonServer.FindClientByTamerId(memberId);
                    //targetMessage.Send(new PartyLeaderChangedPacket(newLeaderSlot).Serialize());
                //}

                _mapServer.BroadcastForTargetTamers(party.GetMembersIdList(), new PartyLeaderChangedPacket(newLeaderSlot).Serialize());
                _dungeonServer.BroadcastForTargetTamers(party.GetMembersIdList(), new PartyLeaderChangedPacket(newLeaderSlot).Serialize());
                _eventServer.BroadcastForTargetTamers(party.GetMembersIdList(), new PartyLeaderChangedPacket(newLeaderSlot).Serialize());
                _pvpServer.BroadcastForTargetTamers(party.GetMembersIdList(), new PartyLeaderChangedPacket(newLeaderSlot).Serialize());

                _logger.Debug($"Tamer {client.TamerId} : {client.Tamer.Name} appointed party slot {newLeaderSlot} as leader.");
            }
            else
            {
                _logger.Warning($"Character {client.TamerId} appointed party leader to slot {newLeaderSlot} but was not in a party.");
            }

            return Task.CompletedTask;
        }
    }
}