using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.Chat;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class HatchRemoveBackupDiskPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.HatchRemoveBackup;

        private readonly AssetsLoader _assets;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public HatchRemoveBackupDiskPacketProcessor(
            AssetsLoader assets,
            ISender sender,
            ILogger logger)
        {
            _assets = assets;
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            if (client.Tamer.Incubator.BackupDiskId > 0)
            {
                var newItem = new ItemModel();
                newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == client.Tamer.Incubator.BackupDiskId));
                newItem.SetItemId(client.Tamer.Incubator.BackupDiskId);
                newItem.SetAmount(1);

                var cloneItem = (ItemModel)newItem.Clone();

                if (client.Tamer.Inventory.AddItem(cloneItem))
                {
                    _logger.Verbose($"Character {client.TamerId} removed backup {client.Tamer.Incubator.BackupDiskId} from incubator to inventory.");
                    await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                }
                else
                {
                    _logger.Warning($"Inventory full for incubator recovery of item {client.Tamer.Incubator.BackupDiskId}.");
                    client.Send(new SystemMessagePacket($"Inventory full for incubator recovery of item {client.Tamer.Incubator.BackupDiskId}."));
                    return;
                }
            }
            else
                _logger.Verbose($"Character {client.TamerId} removed backup {client.Tamer.Incubator.BackupDiskId} from incubator.");

            client.Tamer.Incubator.RemoveBackupDisk();

            await _sender.Send(new UpdateIncubatorCommand(client.Tamer.Incubator));
        }
    }
}