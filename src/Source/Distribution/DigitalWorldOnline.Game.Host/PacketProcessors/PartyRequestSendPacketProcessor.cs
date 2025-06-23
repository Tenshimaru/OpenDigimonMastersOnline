using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartyRequestSendPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartyRequestSend;

        private readonly PartyManager _partyManager;
        private readonly MapServer _mapServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public PartyRequestSendPacketProcessor(
            PartyManager partyManager,
            MapServer mapServer,
            ILogger logger,
            ISender sender)
        {
            _partyManager = partyManager;
            _mapServer = mapServer;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);
            var receiverName = packet.ReadString();

            var targetCharacter = await _sender.Send(new CharacterByNameQuery(receiverName));
            if (targetCharacter == null)
            {
                client.Send(new PartyRequestSentFailedPacket(PartyRequestFailedResultEnum.Disconnected, receiverName));
                _logger.Verbose($"Character {client.TamerId} sent party request to {receiverName} which does not exist.");
            }
            else
            {
                var targetClient = _mapServer.FindClientByTamerId(targetCharacter.Id);
                if (targetClient != null)
                {
                    if (targetClient.Loading || targetClient.Tamer.State != CharacterStateEnum.Ready|| targetClient.DungeonMap)
                    {
                        client.Send(new PartyRequestSentFailedPacket(PartyRequestFailedResultEnum.CantAccept, receiverName));
                        _logger.Verbose($"Character {client.TamerId} sent party request to {targetCharacter.Id} which could not accept.");
                    }                 
                    else
                    {
                        var party = _partyManager.FindParty(targetClient.TamerId);
                        if (party != null)
                        {
                            client.Send(new PartyRequestSentFailedPacket(PartyRequestFailedResultEnum.AlreadyInparty, receiverName));
                            _logger.Verbose($"Character {client.TamerId} sent party request to {targetCharacter.Id} which was already in the party.");
                        }
                        else
                        {
                            targetClient.Send(new PartyRequestSentSuccessPacket(client.Tamer.Name));
                            _logger.Verbose($"Character {client.TamerId} sent party request to {targetCharacter.Id}.");
                        }
                    }
                }
                else
                {
                    client.Send(new PartyRequestSentFailedPacket(PartyRequestFailedResultEnum.Disconnected, receiverName));
                    _logger.Verbose($"Character {client.TamerId} sent party request to {targetCharacter.Id} which was disconnected.");
                }
            }
        }
    }
}