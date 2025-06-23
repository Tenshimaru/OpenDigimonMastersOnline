using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PartnerDigicloneResetPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartnerDigicloneReset;

        private readonly ISender _sender;
        private readonly ILogger _logger;

        public PartnerDigicloneResetPacketProcessor(
            ISender sender,
            ILogger logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var cloneType = (DigicloneTypeEnum)packet.ReadInt();
            var capsuleSlot = packet.ReadInt();

            var capsuleItem = client.Tamer.Inventory.FindItemBySlot(capsuleSlot);
            if (capsuleItem == null)
            {
                _logger.Warning($"Invalid capsule item at slot {capsuleSlot} for tamer {client.TamerId}.");
                client.Send(new SystemMessagePacket($"Invalid capsule item at slot {capsuleSlot}."));
                return;
            }

            if(capsuleItem.ItemInfo.Section == 5701)
            {
                client.Partner.Digiclone.ResetOne(cloneType);
            }
            else
            {
                client.Partner.Digiclone.ResetAll(cloneType);
            }

            var currentCloneLevel = client.Partner.Digiclone.GetCurrentLevel(cloneType);
            _logger.Verbose($"Character {client.TamerId} redefined {client.Partner.Id} clone level to " +
                $"{currentCloneLevel} with {capsuleItem.ItemId}.");

            client.Send(new DigicloneResetPacket(client.Partner.Digiclone));
            client.Send(new UpdateStatusPacket(client.Tamer));

            client.Tamer.Inventory.RemoveOrReduceItem(capsuleItem, 1, capsuleSlot);

            await _sender.Send(new UpdateDigicloneCommand(client.Partner.Digiclone));
            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
        }
    }
}