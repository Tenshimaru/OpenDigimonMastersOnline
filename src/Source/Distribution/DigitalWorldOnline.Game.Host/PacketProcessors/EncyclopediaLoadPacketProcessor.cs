using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Writers;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class EncyclopediaLoadPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.EncyclopediaLoad;

        private readonly ISender _sender;
        private readonly ILogger _logger;

        public EncyclopediaLoadPacketProcessor(ISender sender,ILogger logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var encyclopedia = client.Tamer.Encyclopedia;

            _logger.Debug($"Getting encyclopedia data");

            _logger.Debug($"Encyclopedia's count: {encyclopedia.Count}");

            client.Send(new EncyclopediaLoadPacket(encyclopedia));
        }
    }
}