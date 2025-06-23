using DigitalWorldOnline.Commons.Enums.PacketProcessor;

namespace DigitalWorldOnline.Commons.Interfaces
{
    public interface IGamePacketProcessor : IPacketProcessor
    {
        public GameServerPacketEnum Type { get; }
    }
}
