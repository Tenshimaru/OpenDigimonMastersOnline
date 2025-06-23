using DigitalWorldOnline.Commons.Entities;

namespace DigitalWorldOnline.Commons.Interfaces
{
    public interface IProcessor
    {
        Task ProcessPacketAsync(GameClient client, byte[] data);
    }
}
