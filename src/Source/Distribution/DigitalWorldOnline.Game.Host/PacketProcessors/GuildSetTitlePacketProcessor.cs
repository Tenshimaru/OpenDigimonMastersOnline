using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class GuildSetTitlePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.GuildTitleChange;

        private readonly ILogger _logger;
        private readonly ISender _sender;

        public GuildSetTitlePacketProcessor(
            ILogger logger,
            ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            if (client.Tamer.Guild != null)
            {
                var packet = new GamePacketReader(packetData);

                var type = (GuildAuthorityTypeEnum)packet.ReadInt();
                var newTitle = packet.ReadString();
                packet.Skip(1);
                var newDuty = packet.ReadString();

                var authority = client.Tamer.Guild.UpdateAuthorityTitle(type, newTitle, newDuty);

                //TODO: enviar para os membros?
                _logger.Debug($"Sending guild authority update packet for character {client.TamerId}...");
                client.Send(new GuildAuthorityUpdatePacket(authority));

                _logger.Debug($"Updating guild authority details for guild {client.Tamer.Guild.Id}...");
                await _sender.Send(new UpdateGuildAuthorityCommand(authority));
            }
        }
    }
}