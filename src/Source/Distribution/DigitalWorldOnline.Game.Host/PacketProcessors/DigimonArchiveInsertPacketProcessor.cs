using AutoMapper;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Game.Managers;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class DigimonArchiveInsertPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.DigimonArchiveInsert;

        private readonly StatusManager _statusManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public DigimonArchiveInsertPacketProcessor(
            StatusManager statusManager,
            IMapper mapper,
            ILogger logger,
            ISender sender)
        {
            _statusManager = statusManager;
            _mapper = mapper;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var vipEnabled = Convert.ToBoolean(packet.ReadByte());
            var digiviceSlot = packet.ReadInt();
            var archiveSlot = packet.ReadInt() - 1000;

            var digivicePartner = client.Tamer.Digimons.FirstOrDefault(x => x.Slot == digiviceSlot);
            var archivePartner = client.Tamer.DigimonArchive.DigimonArchives.First(x => x.Slot == archiveSlot);
            //var price = 0;

            if (digivicePartner == null)
            {
                await MoveArchiveToDigivice(client, digiviceSlot, archiveSlot, digivicePartner, archivePartner);
            }
            else if (archivePartner.DigimonId == 0)
            {
                await MovePartnerToArchive(client, digiviceSlot, archiveSlot, digivicePartner, archivePartner, 0);

            }
            else
            {
                await MoveArchiveToDigivice(client, digiviceSlot, archiveSlot, digivicePartner, archivePartner);
                await MovePartnerToArchive(client, digiviceSlot, archiveSlot, digivicePartner, archivePartner, 0);

            }

            client.Send(new DigimonArchiveManagePacket(digiviceSlot, archiveSlot, 0));
        }

        private async Task MoveArchiveToDigivice(
            GameClient client,
            int digiviceSlot,
            int archiveSlot,
            DigimonModel? digivicePartner,
            CharacterDigimonArchiveItemModel archivePartner)
        {
            digivicePartner = _mapper.Map<DigimonModel>(
                await _sender.Send(
                    new GetDigimonByIdQuery(archivePartner.DigimonId)
                )
            );

            digivicePartner.SetBaseInfo(
                _statusManager.GetDigimonBaseInfo(
                    digivicePartner.BaseType
                )
            );

            digivicePartner.SetBaseStatus(
                _statusManager.GetDigimonBaseStatus(
                    digivicePartner.BaseType,
                    digivicePartner.Level,
                    digivicePartner.Size
                )
            );

            digivicePartner.Location = DigimonLocationModel.Create(0, 0, 0);

            digivicePartner.SetSlot((byte)digiviceSlot);
            archivePartner.RemoveDigimon();

            client.Tamer.AddDigimon(digivicePartner);
            client.Send(new UpdateStatusPacket(client.Tamer));

            await _sender.Send(new UpdateCharacterDigimonsOrderCommand(client.Tamer));
            await _sender.Send(new UpdateCharacterDigimonArchiveItemCommand(archivePartner));
        }

        private async Task MovePartnerToArchive(
            GameClient client,
            int digiviceSlot,
            int archiveSlot,
            DigimonModel? digivicePartner,
            CharacterDigimonArchiveItemModel archivePartner,
            int price)
        {
            archivePartner.AddDigimon(digivicePartner.Id);
            digivicePartner.SetSlot(byte.MaxValue);

            client.Tamer.Inventory.RemoveBits(price);

            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateCharacterDigimonsOrderCommand(client.Tamer));
            await _sender.Send(new UpdateCharacterDigimonArchiveItemCommand(archivePartner));

            client.Tamer.RemoveDigimon(byte.MaxValue);
            client.Send(new UpdateStatusPacket(client.Tamer));
            await _sender.Send(new UpdateCharacterDigimonsOrderCommand(client.Tamer));
        }
    }
}