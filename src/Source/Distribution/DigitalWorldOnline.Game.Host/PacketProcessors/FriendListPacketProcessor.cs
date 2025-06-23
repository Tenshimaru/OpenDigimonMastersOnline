using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;
using Serilog;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Packets.GameServer;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class FriendListPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.FriendList;

        private readonly ISender _sender;
        private readonly ILogger _logger;

        public FriendListPacketProcessor(ISender sender, ILogger logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            //_logger.Information("Reading FriendList - packet 2404");

            var friends = client.Tamer.Friends;
            var foes = client.Tamer.Foes;

            //_logger.Information($"Friends: {friends.Count} - Foes: {foes.Count}");

            client.Send(new TamerRelationsPacket(friends, foes));
        }
    }
}