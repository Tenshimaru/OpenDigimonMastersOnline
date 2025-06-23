using DigitalWorldOnline.Commons.Entities;

namespace DigitalWorldOnline.Commons.Interfaces
{
    public interface IPacketProcessor
    {
        public Task Process(GameClient client, byte[] packetData);
    }
}