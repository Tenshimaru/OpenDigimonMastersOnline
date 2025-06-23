using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class AddFriendMemoPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.MemoFriend;

        private readonly ISender _sender;
        private readonly Serilog.ILogger _logger;

        public AddFriendMemoPacketProcessor(ISender sender, Serilog.ILogger logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var friendName = packet.ReadString();
            var oldMemo = packet.ReadString();
            var newMemo = packet.ReadString();

            var friend = client.Tamer.Friends.First(x => x.Name == friendName);

            if (friend != null) await _sender.Send(new ChangeFriendMemoCommand(friend.Id, newMemo));

        }
    }
}