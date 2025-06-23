using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class NpcRepurchaseListPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.LoadNpcRepurchaseList;

        private readonly ILogger _logger;

        public NpcRepurchaseListPacketProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);
            var npcId = packet.ReadInt();

            client.Send(new NpcRepurchaseListPacket(client.Tamer.RepurchaseList));

            _logger.Verbose($"Character {client.TamerId} openned NPC {npcId} store.");

            return Task.CompletedTask;
        }
    }
}