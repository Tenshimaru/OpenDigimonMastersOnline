using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Packets.GameServer;
using MediatR;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class AddFriendPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.AddFriend;

        private readonly ISender _sender;

        public AddFriendPacketProcessor(ISender sender)
        {
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var friendName = packet.ReadString();
            byte status = 0;
            bool stat = false;

            var availableName = await _sender.Send(new CharacterByNameQuery(friendName));

            if (availableName == null)
            {
                client.Send(new FriendNotFoundPacket(friendName));
            }
            else
            {
                if (availableName.State == CharacterStateEnum.Disconnected)
                {
                    status = 0;
                    stat = true;
                }
                else
                {
                    status = 1;
                }

                var newFriend = CharacterFriendModel.Create(friendName, availableName.Id, stat);

                newFriend.SetTamer(client.Tamer);
                newFriend.Annotation = "";
                client.Tamer.AddFriend(newFriend);

                var FriendInfo = await _sender.Send(new CreateNewFriendCommand(newFriend));

                if (FriendInfo != null)
                {
                    client.Send(new AddFriendPacket(friendName, status));
                }

            }
        }
    }
}