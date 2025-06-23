using AutoMapper;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Writers;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class DigimonArchiveSwapPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.DigimonArchiveSwap;

        private readonly StatusManager _statusManager;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public DigimonArchiveSwapPacketProcessor(
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
            var pack = new GamePacketReader(packetData);

            var vipEnabled = Convert.ToBoolean(pack.ReadByte());

            var OldSlot = pack.ReadInt() - 1000;
            var NewSlot = pack.ReadInt() - 1000;
            int npcId = pack.ReadInt();


            // Verificar se os slots estão dentro dos limites válidos
            if (OldSlot >= 0 && NewSlot >= 0)
            {
                var Receiver = client.Tamer.DigimonArchive.DigimonArchives.FirstOrDefault(x => x.Slot == NewSlot);
                var Older = client.Tamer.DigimonArchive.DigimonArchives.FirstOrDefault(x => x.Slot == OldSlot);
                var ReceiverB = Receiver.DigimonId;
                var OlderB = Older.DigimonId;

                // Verificar se os Digimons existem nos slots especificados
                if (Receiver != null && Older != null)
                {
                    //Limpar Slots
                    Receiver.RemoveDigimon();
                    Older.RemoveDigimon();
                    // Trocar os slots
                    Receiver.AddDigimon(OlderB);
                    Older.AddDigimon(ReceiverB);


                    await _sender.Send(new UpdateCharacterDigimonArchiveItemCommand(Older));
                    await _sender.Send(new UpdateCharacterDigimonArchiveItemCommand(Receiver));

                }
                /*var packet = new PacketWriter();
                packet.Type(3243);
                packet.WriteInt(OldSlot + 1000);
                packet.WriteInt(NewSlot + 1000);

                client.Send(packet.Serialize());*/
                client.Send(new DigimonArchivePacket(OldSlot, NewSlot));
            }
        }
    }
}