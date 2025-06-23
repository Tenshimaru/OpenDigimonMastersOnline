using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Microsoft.Identity.Client;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class HatchIncreasePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.HatchIncrease;

        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly AssetsLoader _assets;
        private readonly ConfigsLoader _configs;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public HatchIncreasePacketProcessor(
            MapServer mapServer,
            DungeonsServer dungeonsServer,
            EventServer eventServer,
            PvpServer pvpServer,
            AssetsLoader assets,
            ConfigsLoader configs,
            ILogger logger,
            ISender sender
        )
        {
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _assets = assets;
            _configs = configs;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var vipEnabled = packet.ReadByte();
            var npcId = packet.ReadInt();
            var dataTier = packet.ReadByte();

            //_logger.Information($"Data Type: {dataTier} (0 - Low, 1 - Mid)");

            var targetItem = client.Tamer.Incubator.EggId;

            if (targetItem == 0)
            {
                _logger.Error($"targetItem not found !!");
                return;
            }

            var itemInfo = _assets.ItemInfo.FirstOrDefault(x => x.ItemId == targetItem);

            if (itemInfo == null)
            {
                _logger.Error($"ItemInfo not found !!");
                return;
            }

            var hatchInfo = _assets.Hatchs.FirstOrDefault(x => x.ItemId == targetItem);

            if (hatchInfo == null)
            {
                _logger.Warning($"Unknown hatch info for egg {targetItem}.");
                client.Send(new SystemMessagePacket($"Unknown hatch info for egg {targetItem}."));
                return;
            }

            var hatchConfig = _configs.Hatchs.FirstOrDefault(x => x.Type.GetHashCode() == client.Tamer.Incubator.HatchLevel + 1);

            if (hatchConfig == null)
            {
                client.Send(new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler,
                    HatchIncreaseResultEnum.Failled));
                _logger.Error($"Invalid hatch config for level {client.Tamer.Incubator.HatchLevel + 1}.");
                client.Send(
                    new SystemMessagePacket(
                        $"Invalid hatch config for level {client.Tamer.Incubator.HatchLevel + 1}."));
                return;
            }

            var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

            // Mid Data use verification
            /*if ((client.Tamer.Incubator.HatchLevel + 1) > hatchInfo.LowClassLimitLevel && dataTier == 0)
            {
                client.Send(new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled));
                client.Send(new SystemMessagePacket($"Data insert Failed ! Use MidData to continue ..."));
                _logger.Information($"Data insert Failed ! Use MidData to continue ...");
                return;
            }*/

            if (dataTier == 0)
            {
                //_logger.Information($"Using LowClass Data !!");

                var success = client.Tamer.Inventory.RemoveOrReduceItemsBySection(hatchInfo.LowClassDataSection, hatchInfo.LowClassDataAmount);

                if (!success)
                {
                    client.Send(new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled));

                    _logger.Error($"Invalid low class data amount for egg {targetItem} and section {hatchInfo.LowClassDataSection}.");
                    client.Send(new SystemMessagePacket($"Invalid low class data amount for egg {targetItem} and section {hatchInfo.LowClassDataSection}."));

                    //sistema de banimento permanente
                    var banProcessor = SingletonResolver.GetService<BanForCheating>();
                    var banMessage = banProcessor.BanAccountWithMessage(client.AccountId, client.Tamer.Name, AccountBlockEnum.Permanent, "Cheating", client, "You tried to hatch a digimon using a cheat method, So be happy with ban!");

                    var chatPacket = new NoticeMessagePacket(banMessage);
                    client.Send(chatPacket); // Envia a mensagem no chat

                    // client.Send(new DisconnectUserPacket($"YOU HAVE BEEN PERMANENTLY BANNED").Serialize());

                    return;
                }
                else
                {
                    if (client.Tamer.Incubator.HatchLevel < hatchInfo.MidClassBreakPoint && itemInfo.Class != 4 &&
                        (hatchInfo.LowClassBreakPoint == hatchInfo.MidClassBreakPoint || hatchInfo.LowClassLimitLevel == hatchInfo.MidClassBreakPoint))
                    {
                        client.Tamer.Incubator.IncreaseLevel();

                        switch (mapConfig?.Type)
                        {
                            case MapTypeEnum.Dungeon:
                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId, new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                break;

                            case MapTypeEnum.Event:
                                _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId, new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                break;

                            case MapTypeEnum.Pvp:
                                _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId, new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                break;

                            default:
                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                break;
                        }
                    }
                    else
                    {
                        _logger.Debug($"Normal Egg !!");

                        if (hatchConfig.SuccessChance >= UtilitiesFunctions.RandomDouble())
                        {
                            client.Tamer.Incubator.IncreaseLevel();

                            switch (mapConfig?.Type)
                            {
                                case MapTypeEnum.Dungeon:
                                    _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                    break;

                                case MapTypeEnum.Event:
                                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                    break;

                                case MapTypeEnum.Pvp:
                                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                    break;

                                default:
                                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                    break;
                            }

                            _logger.Verbose($"Tamer {client.TamerId}:{client.Tamer.Name} succeeded to increase egg {targetItem} to level {client.Tamer.Incubator.HatchLevel}" +
                                $"with data section {hatchInfo.LowClassDataSection} x{hatchInfo.LowClassDataAmount}.");
                        }
                        else
                        {
                            if (hatchConfig.BreakChance >= UtilitiesFunctions.RandomDouble())
                            {
                                if (client.Tamer.Incubator.BackupDiskId > 0)
                                {
                                    switch (mapConfig?.Type)
                                    {
                                        case MapTypeEnum.Dungeon:
                                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Backuped).Serialize());
                                            break;

                                        case MapTypeEnum.Event:
                                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Backuped).Serialize());
                                            break;

                                        case MapTypeEnum.Pvp:
                                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Backuped).Serialize());
                                            break;

                                        default:
                                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Backuped).Serialize());
                                            break;
                                    }

                                    _logger.Verbose($"Character {client.TamerId} failed to increase egg {targetItem} to level {client.Tamer.Incubator.HatchLevel + 1} " +
                                        $"with data section {hatchInfo.MidClassDataSection} x{hatchInfo.MidClassDataAmount} and egg was saved by {client.Tamer.Incubator.BackupDiskId}.");
                                }
                                else
                                {
                                    switch (mapConfig?.Type)
                                    {
                                        case MapTypeEnum.Dungeon:
                                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Broken).Serialize());
                                            break;

                                        case MapTypeEnum.Event:
                                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Broken).Serialize());
                                            break;

                                        case MapTypeEnum.Pvp:
                                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Broken).Serialize());
                                            break;

                                        default:
                                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Broken).Serialize());
                                            break;
                                    }

                                    _logger.Verbose($"Character {client.TamerId} failed to increase egg {targetItem} to level {client.Tamer.Incubator.HatchLevel + 1} " +
                                        $"with data section {hatchInfo.MidClassDataSection} x{hatchInfo.MidClassDataAmount} and egg has broken.");

                                    client.Tamer.Incubator.RemoveEgg();
                                }
                            }
                            else
                            {
                                switch (mapConfig?.Type)
                                {
                                    case MapTypeEnum.Dungeon:
                                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled).Serialize());
                                        break;

                                    case MapTypeEnum.Event:
                                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled).Serialize());
                                        break;

                                    case MapTypeEnum.Pvp:
                                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled).Serialize());
                                        break;

                                    default:
                                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled).Serialize());
                                        break;
                                }

                                _logger.Verbose($"Character {client.TamerId} failed to increase egg {targetItem} to level {client.Tamer.Incubator.HatchLevel + 1} " +
                                    $"with data section {hatchInfo.MidClassDataSection} x{hatchInfo.MidClassDataAmount}.");
                            }
                        }
                    }
                }
            }
            else
            {
                //_logger.Information($"Using MidClass Data !!");

                var success = client.Tamer.Inventory.RemoveOrReduceItemsBySection(hatchInfo.MidClassDataSection, hatchInfo.MidClassDataAmount);

                if (!success)
                {
                    client.Send(new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled));

                    _logger.Error($"Invalid mid class data amount for egg {targetItem} and section {hatchInfo.MidClassDataSection}.");
                    client.Send(new SystemMessagePacket($"Invalid mid class data amount for egg {targetItem} and section {hatchInfo.MidClassDataSection}."));
                    return;
                }
                else
                {
                    if (client.Tamer.Incubator.HatchLevel < hatchInfo.MidClassBreakPoint && itemInfo.Class != 4)
                    {
                        //_logger.Information($"Perfect Egg !!");

                        client.Tamer.Incubator.IncreaseLevel();

                        switch (mapConfig?.Type)
                        {
                            case MapTypeEnum.Dungeon:
                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId, new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                break;

                            case MapTypeEnum.Event:
                                _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId, new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                break;

                            case MapTypeEnum.Pvp:
                                _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId, new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                break;

                            default:
                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId, new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                break;
                        }
                    }
                    else
                    {
                        //_logger.Information($"Normal Egg !!");

                        if (hatchConfig.SuccessChance >= UtilitiesFunctions.RandomDouble())
                        {
                            client.Tamer.Incubator.IncreaseLevel();

                            switch (mapConfig?.Type)
                            {
                                case MapTypeEnum.Dungeon:
                                    _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                    break;

                                case MapTypeEnum.Event:
                                    _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                    break;

                                case MapTypeEnum.Pvp:
                                    _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                    break;

                                default:
                                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new HatchIncreaseSucceedPacket(client.Tamer.GeneralHandler, client.Tamer.Incubator.HatchLevel).Serialize());
                                    break;
                            }

                            _logger.Verbose($"Tamer {client.TamerId}:{client.Tamer.Name} succeeded to increase egg {targetItem} to level {client.Tamer.Incubator.HatchLevel}" +
                                $"with data section {hatchInfo.LowClassDataSection} x{hatchInfo.LowClassDataAmount}.");
                        }
                        else
                        {
                            if (hatchConfig.BreakChance >= UtilitiesFunctions.RandomDouble())
                            {
                                if (client.Tamer.Incubator.BackupDiskId > 0)
                                {
                                    switch (mapConfig?.Type)
                                    {
                                        case MapTypeEnum.Dungeon:
                                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Backuped).Serialize());
                                            break;

                                        case MapTypeEnum.Event:
                                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Backuped).Serialize());
                                            break;

                                        case MapTypeEnum.Pvp:
                                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Backuped).Serialize());
                                            break;

                                        default:
                                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Backuped).Serialize());
                                            break;
                                    }

                                    _logger.Verbose($"Character {client.TamerId} failed to increase egg {targetItem} to level {client.Tamer.Incubator.HatchLevel + 1} " +
                                        $"with data section {hatchInfo.MidClassDataSection} x{hatchInfo.MidClassDataAmount} and egg was saved by {client.Tamer.Incubator.BackupDiskId}.");
                                }
                                else
                                {
                                    switch (mapConfig?.Type)
                                    {
                                        case MapTypeEnum.Dungeon:
                                            _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Broken).Serialize());
                                            break;

                                        case MapTypeEnum.Event:
                                            _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Broken).Serialize());
                                            break;

                                        case MapTypeEnum.Pvp:
                                            _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Broken).Serialize());
                                            break;

                                        default:
                                            _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                                new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Broken).Serialize());
                                            break;
                                    }

                                    _logger.Verbose($"Character {client.TamerId} failed to increase egg {targetItem} to level {client.Tamer.Incubator.HatchLevel + 1} " +
                                        $"with data section {hatchInfo.MidClassDataSection} x{hatchInfo.MidClassDataAmount} and egg has broken.");

                                    client.Tamer.Incubator.RemoveEgg();
                                }
                            }
                            else
                            {
                                switch (mapConfig?.Type)
                                {
                                    case MapTypeEnum.Dungeon:
                                        _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled).Serialize());
                                        break;

                                    case MapTypeEnum.Event:
                                        _eventServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled).Serialize());
                                        break;

                                    case MapTypeEnum.Pvp:
                                        _pvpServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled).Serialize());
                                        break;

                                    default:
                                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new HatchIncreaseFailedPacket(client.Tamer.GeneralHandler, HatchIncreaseResultEnum.Failled).Serialize());
                                        break;
                                }

                                _logger.Verbose($"Character {client.TamerId} failed to increase egg {targetItem} to level {client.Tamer.Incubator.HatchLevel + 1} " +
                                    $"with data section {hatchInfo.MidClassDataSection} x{hatchInfo.MidClassDataAmount}.");
                            }
                        }
                    }
                }
            }

            client.Tamer.Incubator.RemoveBackupDisk();

            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateIncubatorCommand(client.Tamer.Incubator));
        }
    }
}