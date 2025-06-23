using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class BurningEventPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.BurningEvent;

        private readonly ISender _sender;
        private readonly ILogger _logger;

        public BurningEventPacketProcessor(ISender sender, ILogger logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            _logger.Verbose($"--- Burning Event Packet 3132 ---\n");

            uint m_nExpRate = packet.ReadUInt();
            uint m_nNextDayExpRate = packet.ReadUInt();
            uint m_nExpTarget = packet.ReadUInt();
            uint m_nSpecialExp = packet.ReadUInt();

            _logger.Verbose($"ExpRate: {m_nExpRate} | NextDayExpRate: {m_nNextDayExpRate} | ExpTarget: {m_nExpTarget}\n");

            _logger.Verbose($"---------------------------------");

            //await _sender.Send(new BurningEventPacket(m_nExpRate, m_nNextDayExpRate, m_nExpTarget, m_nSpecialExp));

        }
    }
}
