using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.GameHost;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Reflection;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ChannelsPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.Channels;
        
        private readonly MapServer _mapServer;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public ChannelsPacketProcessor(ISender sender, ILogger logger, MapServer mapServer)
        {
            _sender = sender;
            _logger = logger;
            _mapServer = mapServer;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            _logger.Debug($"Getting available channels...");

            if (!client.DungeonMap)
            {
                var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

                int channelsCount;

                if(mapConfig != null)
                {
                    channelsCount = mapConfig.Channels;
                }
                else
                {
                    channelsCount = 3;
                }

                // -1 => unused
                // 0:smooth
                // 1:many
                // 2:too many
                // 3~10:congested
                var channels = new Dictionary<byte, byte>();

                var mapChannels = await _sender.Send(new ChannelsByMapIdQuery(client.Tamer.Location.MapId));
                var currentChannel = mapChannels.Where(x => x.Key == client.Tamer.Channel).FirstOrDefault();

                if (mapChannels != null)
                {
                    foreach (var channel in mapChannels.OrderBy(x => x.Key))
                    {
                        channels.Add(channel.Key, channel.Value);
                    }
                }
                else
                {
                    for (byte i = 0; i < channelsCount; i++)
                    {
                        channels.Add(i, 0);
                    }
                }

                if (!channels.IsNullOrEmpty())
                {
                    client.Send(new AvailableChannelsPacket(channels).Serialize());
                }
            }
            else
            {
                var channels = new Dictionary<byte, byte>
                {
                    { 0, 30 }
                };
            }

            //return Task.CompletedTask;
        }
    }
}