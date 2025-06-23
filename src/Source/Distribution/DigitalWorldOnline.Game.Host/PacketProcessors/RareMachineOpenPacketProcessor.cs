using DigitalWorldOnline.Application;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class RareMachineOpenPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.RareMachineOpen;

        private readonly AssetsLoader _assets;
        private readonly ILogger _logger;

        public RareMachineOpenPacketProcessor(AssetsLoader assets, ILogger logger)
        {
            _assets = assets;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var NpcId = packet.ReadInt();

            //_logger.Information($"Gotcha Npc: {NpcId}");

            var Gotcha = _assets.Gotcha.FirstOrDefault(x => x.NpcId == NpcId);

            client.Send(new GotchaStartPacket(Gotcha));
        }
    }
}
