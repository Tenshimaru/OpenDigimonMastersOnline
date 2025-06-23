using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.PersonalShop;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ConsignedWarehousePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ConsignedWarehouse;

        private readonly ILogger _logger;

        public ConsignedWarehousePacketProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public Task Process(GameClient client, byte[] packetData)
        {
            _logger.Debug($"Sending consigned warehouse packet...");
            client.Send(new LoadConsignedShopWarehousePacket(client.Tamer.ConsignedWarehouse));

            return Task.CompletedTask;
        }
    }
}