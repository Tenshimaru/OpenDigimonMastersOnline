using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Enums.Party;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartyChangeLootTypePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartyConfigChange;

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly ILogger _logger;

        public PartyChangeLootTypePacketProcessor(
            PartyManager partyManager,
            MapServer mapServer,
            DungeonsServer dungeonServer,
            ILogger logger)
        {
            _partyManager = partyManager;
            _mapServer = mapServer;
            _dungeonServer = dungeonServer;
            _logger = logger;
        }


        public Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);
            var lootType = (PartyLootShareTypeEnum)packet.ReadInt();
            var rareType = (PartyLootShareRarityEnum)packet.ReadByte();

            var party = _partyManager.FindParty(client.TamerId);

            if(party != null)
            {
                party.ChangeLootType(lootType, rareType);

                foreach (var target in party.Members.Values)
                {

                    var targetClient = _mapServer.FindClientByTamerId(target.Id);
                    if (targetClient == null) targetClient = _dungeonServer.FindClientByTamerId(target.Id);

                    if (targetClient == null)
                        continue;

                    targetClient.Send(new PartyChangeLootTypePacket(lootType, rareType));
                }
            }
            return Task.CompletedTask;
        }
    }
}