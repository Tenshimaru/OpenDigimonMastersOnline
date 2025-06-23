using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Account;
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
    public class PartnerDigiclonePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.PartnerDigiclone;

        private readonly MapServer _mapServer;
        private readonly AssetsLoader _assets;
        private readonly DungeonsServer _dungeonServer;
        private readonly ConfigsLoader _configs;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public PartnerDigiclonePacketProcessor(
            MapServer mapServer,
            AssetsLoader assets,
            ConfigsLoader configs,
            ISender sender,
            ILogger logger,
            DungeonsServer dungeonsServer)
        {
            _mapServer = mapServer;
            _assets = assets;
            _configs = configs;
            _sender = sender;
            _logger = logger;
            _dungeonServer = dungeonsServer;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var cloneType = (DigicloneTypeEnum)packet.ReadInt();
            var digicloneSlot = packet.ReadInt();
            var backupSlot = packet.ReadInt();

            var digicloneItem = client.Tamer.Inventory.FindItemBySlot(digicloneSlot);
            if (digicloneItem == null)
            {
                _logger.Warning($"Invalid clone item at slot {digicloneSlot} for tamer {client.TamerId}.");
                client.Send(new SystemMessagePacket($"Invalid clone item at slot {digicloneSlot}."));
                return;
            }

            _logger.Information(
                $"Digiclone Digimon Id: {client.Partner.Digiclone?.DigimonId}, Digiclone Id: {client.Partner.Digiclone?.Id}");

            var currentCloneLevel = client.Partner.Digiclone.GetCurrentLevel(cloneType);

            var cloneConfig =
                _configs.Clones.FirstOrDefault(x => x.Type == cloneType && x.Level == currentCloneLevel + 1);

            if (cloneConfig == null)
            {
                _logger.Warning($"Invalid clone config with type {cloneType} and level {currentCloneLevel}.");
                client.Send(new SystemMessagePacket($"Invalid clone config."));
                return;
            }

            var clonePriceAsset = _assets.Clones.FirstOrDefault(x => x.ItemSection == digicloneItem.ItemInfo.Section);

            if (clonePriceAsset == null)
            {
                _logger.Information($"Invalid clone price assets with item section {digicloneItem.ItemInfo.Section}.");
                client.Send(UtilitiesFunctions.GroupPackets(
                    new SystemMessagePacket($"Invalid clone assets.").Serialize(),
                    new DigicloneResultPacket(DigicloneResultEnum.Fail, client.Partner.Digiclone).Serialize()
                ));
                return;
            }

            // Validate item as clone item and equals min and max level
            if (!(cloneConfig.Level <= clonePriceAsset.MaxLevel && cloneConfig.Level >= clonePriceAsset.MinLevel) ||
                !UtilitiesFunctions.IsCloneItem(digicloneItem.ItemInfo.Section))
            {
                client.Send(new DigicloneResultPacket(DigicloneResultEnum.Fail, client.Partner.Digiclone).Serialize());
                client.SendToAll(new NoticeMessagePacket(
                        $"Tamer: {client.Tamer.Name} tried to clone their digimon using a cheat method, Then they got banned!")
                    .Serialize());

                var banProcessor = SingletonResolver.GetService<BanForCheating>();
                banProcessor.BanAccountWithMessage(client.AccountId, client.Tamer.Name,
                    AccountBlockEnum.Permanent, "Cheating", client, "You tried to clone your digimon using a cheat method, So be happy with ban!");

                _logger.Warning(
                    $"Tamer: {client.Tamer.Name} tried to clone their using a cheat method, Then they got banned!");
                /*client.Send(
                    new DisconnectUserPacket(
                        $"You tried to change starter digimon size using a cheat method, so be happy with Ban").Serialize());*/
                return;
            }

            var cloneAsset = _assets.CloneValues.FirstOrDefault(x =>
                x.Type == cloneType && currentCloneLevel + 1 >= x.MinLevel && currentCloneLevel + 1 <= x.MaxLevel);

            if (cloneAsset == null)
            {
                _logger.Information($"Invalid clone assets with type {cloneType} and level {currentCloneLevel}.");
                client.Send(UtilitiesFunctions.GroupPackets(
                    new SystemMessagePacket($"Invalid clone assets.").Serialize(),
                    new DigicloneResultPacket(DigicloneResultEnum.Fail, client.Partner.Digiclone).Serialize()
                ));
                return;
            }

            var cloneResult = DigicloneResultEnum.Fail;
            short value = 0;

            if (clonePriceAsset.Reinforced)
            {
                if (cloneConfig.SuccessChance >= UtilitiesFunctions.RandomDouble())
                {
                    cloneResult = DigicloneResultEnum.Success;
                    value = (short)cloneAsset.MaxValue;
                }
            }
            else if (clonePriceAsset.Mega)
            {
                cloneResult = DigicloneResultEnum.Success;
                value = UtilitiesFunctions.RandomShort((short)cloneAsset.MinValue, (short)(cloneAsset.MaxValue));
            }
            else if (clonePriceAsset.MegaReinforced)
            {
                cloneResult = DigicloneResultEnum.Success;
                value = (short)cloneAsset.MaxValue;
            }
            else if (clonePriceAsset.Low)
            {
                cloneResult = DigicloneResultEnum.Success;
                value = (short)cloneAsset.MinValue;
            }
            else
            {
                if (cloneConfig.SuccessChance >= UtilitiesFunctions.RandomDouble())
                {
                    cloneResult = DigicloneResultEnum.Success;
                    value = UtilitiesFunctions.RandomShort((short)cloneAsset.MinValue, (short)(cloneAsset.MaxValue));
                }
            }

            var backupItem = client.Tamer.Inventory.FindItemBySlot(backupSlot);

            if (cloneResult == DigicloneResultEnum.Success)
            {
                _logger.Information(
                    $"Character {client.TamerId} increased {client.Partner.Id} {cloneType} clon level to " +
                    $"{currentCloneLevel + 1} with value {value} using {digicloneItem.ItemId} {backupItem?.ItemId}.");

                client.Partner.Digiclone.IncreaseCloneLevel(cloneType, value);

                _logger.Information($"Increased clone level with history");
                if (client.Partner.Digiclone.MaxCloneLevel)
                {
                    client.SendToAll(new NeonMessagePacket(
                            NeonMessageTypeEnum.Digimon,
                            client.Tamer.Name,
                            client.Partner.CurrentType,
                            client.Partner.Digiclone.CloneLevel - 1
                        )
                        .Serialize());
                }
            }
            else
            {
                if (cloneConfig.CanBreak)
                {
                    if (cloneConfig.BreakChance >= UtilitiesFunctions.RandomDouble())
                    {
                        if (backupItem == null)
                        {
                            _logger.Information(
                                $"Character {client.TamerId} broken {client.Partner.Id} {cloneType} clon level to " +
                                $"{currentCloneLevel - 1} using {digicloneItem.ItemId} without backup.");

                            cloneResult = DigicloneResultEnum.Break;
                            client.Partner.Digiclone.Break(cloneType);
                        }
                        else
                        {
                            _logger.Information(
                                $"Character {client.TamerId} failed to increase {client.Partner.Id} {cloneType} clon level to " +
                                $"{currentCloneLevel + 1} using {digicloneItem.ItemId} with backup {backupItem?.ItemId}.");

                            cloneResult = DigicloneResultEnum.Backup;
                        }
                    }
                }
                else
                {
                    _logger.Information(
                        $"Character {client.TamerId} failed to increase {client.Partner.Id} {cloneType} clon level to " +
                        $"{currentCloneLevel + 1} using {digicloneItem.ItemId} {backupItem?.ItemId}.");

                    cloneResult = DigicloneResultEnum.Fail;
                }
            }

            _logger.Information($"6666666666666666");

            client.Send(new DigicloneResultPacket(cloneResult, client.Partner.Digiclone));
            if (cloneResult == DigicloneResultEnum.Success)
            {
                client.Send(new UpdateStatusPacket(client.Tamer));
            }

            _logger.Information($"7777777777777777");
            client.Tamer.Inventory.RemoveBits(clonePriceAsset.Bits);
            client.Tamer.Inventory.RemoveOrReduceItem(digicloneItem, 1, digicloneSlot);
            client.Tamer.Inventory.RemoveOrReduceItem(backupItem, 1, backupSlot);
            _logger.Information($"Character {client.TamerId} removed {clonePriceAsset.Bits} bits from inventory.");
            _logger.Information(
                $"Digimon Digiclone: {client.Partner.Digiclone?.Id}, DigimonId: {client.Partner.Digiclone?.DigimonId}");
            await _sender.Send(new UpdateDigicloneCommand(client.Partner.Digiclone));
            _logger.Information($"Character {client.TamerId} saved digiclone info");
            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));
            _logger.Information($"Character {client.TamerId} saved bits info");
            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            _logger.Information($"Character {client.TamerId} saved items info");
        }
    }
}