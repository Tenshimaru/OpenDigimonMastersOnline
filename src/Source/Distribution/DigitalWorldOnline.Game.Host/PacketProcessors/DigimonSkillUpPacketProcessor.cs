using DigitalWorldOnline.Application;
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
    public class DigimonSkillUpPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.SkillLevelUp;

        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly AssetsLoader _assets;

        public DigimonSkillUpPacketProcessor(
            ILogger logger,
            ISender sender,
            AssetsLoader assets)
        {
            _logger = logger;
            _sender = sender;
            _assets = assets;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {

            var packet = new GamePacketReader(packetData);
            int GeneralHandler = packet.ReadInt();
            byte formSlot = packet.ReadByte();
            byte skillSlot = packet.ReadByte();

            var Evolution = client.Tamer.Partner.Evolutions[formSlot -1];

            if (Evolution != null)
            {
                var TargetEvolution = _assets.DigimonSkillInfo.FirstOrDefault(x => x.Type == client.Partner.CurrentType && x.Slot == skillSlot);

                if (TargetEvolution != null)
                {
                    Evolution.DecreaseSkillPoints(TargetEvolution.SkillInfo.RequiredPoints);
                    Evolution.Skills[skillSlot].IncreaseSkillLevel();
                    _logger.Debug($"partner increase skill level to{Evolution.Skills[skillSlot].CurrentLevel} and consume points{TargetEvolution.SkillInfo.RequiredPoints}...");

                    await _sender.Send(new UpdateEvolutionCommand(Evolution));
                }
            }


        }
    }
}