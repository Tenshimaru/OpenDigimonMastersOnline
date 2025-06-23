using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class FriendInformationPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.FriendInformation;

        public FriendInformationPacketProcessor()
        {
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            //TODO: ajeitar
            client.Send(new FriendInformationPacket());
        }
    }
}