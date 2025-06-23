using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.GameServer;
using MediatR;
using Serilog;
using System.Net.Sockets;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class DigimonTranscendenceSuccessPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TranscendenceSuccess;

        private readonly ILogger _logger;
        private readonly ISender _sender;

        public DigimonTranscendenceSuccessPacketProcessor(
            ILogger logger,
            ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            int Result = packet.ReadInt();
            byte targetSlot = packet.ReadByte();
            int NpcId = packet.ReadInt();
            byte targetAcademySlot = packet.ReadByte();
            long price = packet.ReadInt64();

            int exp = 140000;       // Transcendence Exp Needed (140000 = 100%)

            var targetPartner = client.Tamer.Digimons.FirstOrDefault(x => x.Slot == targetAcademySlot);

            if (targetPartner == null)
                return;

            if (targetPartner.PossibleTranscendence)
            {
                client.Tamer.Inventory.RemoveBits(price);
                targetPartner.Transcend();

                client.Send(new DigimonTranscendenceSuccessPacket(Result, targetAcademySlot, targetPartner.HatchGrade, price, (int)client.Tamer.Inventory.Bits, exp));
                await _sender.Send(new UpdateDigimonGradeCommand(targetPartner.Id, targetPartner.HatchGrade));
            }
            else
            {
                _logger.Warning($"Transcendence not possible, Exp not 100% !!");
                client.Send(new DigimonTranscendenceSuccessPacket(Result, targetAcademySlot, targetPartner.HatchGrade, price, (int)client.Tamer.Inventory.Bits, exp));
            }
        }
    }
}