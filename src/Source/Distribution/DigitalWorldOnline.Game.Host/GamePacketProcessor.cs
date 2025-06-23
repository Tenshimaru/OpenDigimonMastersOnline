using DigitalWorldOnline.Application;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using Serilog;

namespace DigitalWorldOnline.Game
{
    public sealed partial class GamePacketProcessor : IProcessor, IDisposable
    {
        private readonly IEnumerable<IGamePacketProcessor> _packetProcessors;
        private readonly AssetsLoader _assets;
        private readonly ConfigsLoader _configs;
        private readonly ILogger _logger;

        public GamePacketProcessor(
            IEnumerable<IGamePacketProcessor> packetProcessors,
            AssetsLoader assets,
            ConfigsLoader configs,
            ILogger logger)
        {
            _packetProcessors = packetProcessors;
            _assets = assets;
            _configs = configs;
            _logger = logger;
        }

        /// <summary>
        /// Process the arrived TCP packet, sent from the game client
        /// </summary>
        /// <param name="client">The game client whos sent the packet</param>
        /// <param name="data">The packet bytes array</param>
        public async Task ProcessPacketAsync(GameClient client, byte[] data)
        {
            while (_assets.Loading || _configs.Loading) await Task.Delay(1000);

            var packet = new GamePacketReader(data);

            switch (packet.Enum)
            {
                case GameServerPacketEnum.Unknown:
                    _logger.Warning($"Unknown packet. Type: {packet.Type}. Length: {packet.Length}.");
                    break;

                default:
                    {
                        var processor = _packetProcessors.FirstOrDefault(x => x.Type == packet.Enum);

                        if (processor != null)
                        {
                            await processor.Process(client, data);
                        }
                        else
                        {
                            _logger.Error($"No processor for packet {packet.Type} to player {client.Tamer.Name}.");
                            //throw new NotImplementedException();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Disposes the entire object.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
