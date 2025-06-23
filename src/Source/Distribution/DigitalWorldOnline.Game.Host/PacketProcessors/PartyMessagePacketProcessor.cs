using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Chat;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartyMessagePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartyMessage;

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public PartyMessagePacketProcessor(PartyManager partyManager, MapServer mapServer, DungeonsServer dungeonsServer,
            EventServer eventServer, PvpServer pvpServer, ILogger logger, ISender sender)
        {
            _partyManager = partyManager;
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var message = packet.ReadString();

            var party = _partyManager.FindParty(client.TamerId);

            if (party == null)
            {
                client.Send(new SystemMessagePacket($"You need to be in a party to send party messages."));
                _logger.Verbose($"Character {client.TamerId} sent party message but was not in a party.");
                return;
            }
            
            foreach (var memberId in party.GetMembersIdList())
            {
                var targetMessage = _mapServer.FindClientByTamerId(memberId);
                var targetDungeon = _dungeonServer.FindClientByTamerId(memberId);
                var targetEvent = _eventServer.FindClientByTamerId(memberId);
                var targetPvp = _pvpServer.FindClientByTamerId(memberId);

                if (targetMessage != null)
                    targetMessage.Send(new PartyMessagePacket(client.Tamer.Name, message).Serialize());
                
                if (targetDungeon != null)
                    targetDungeon.Send(new PartyMessagePacket(client.Tamer.Name, message).Serialize());

                if (targetEvent != null)
                    targetEvent.Send(new PartyMessagePacket(client.Tamer.Name, message).Serialize());

                if (targetPvp != null)
                    targetPvp.Send(new PartyMessagePacket(client.Tamer.Name, message).Serialize());

            }

            _logger.Verbose($"Character {client.TamerId} sent chat to party {party.Id} with message {message}.");

            await _sender.Send(new CreateChatMessageCommand(ChatMessageModel.Create(client.TamerId, message)));
        }
    }
}