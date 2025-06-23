using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Delete;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.GameServer.Combat;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text.RegularExpressions;
using DigitalWorldOnline.Game.PacketProcessors;

namespace DigitalWorldOnline.Game
{
    public sealed class GameMasterCommandsProcessor : IDisposable
    {
        private const string GameServerAddress = "GameServer:Address";
        private const string GamerServerPublic = "GameServer:PublicAddress";
        private const string GameServerPort = "GameServer:Port";

        private readonly PartyManager _partyManager;
        private readonly StatusManager _statusManager;
        private readonly ExpManager _expManager;
        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GameMasterCommandsProcessor(
            PartyManager partyManager,
            StatusManager statusManager,
            ExpManager expManager,
            AssetsLoader assets,
            MapServer mapServer,
            DungeonsServer dungeonsServer,
            EventServer eventServer,
            PvpServer pvpServer,
            ILogger logger,
            ISender sender,
            IMapper mapper,
            IConfiguration configuration)
        {
            _partyManager = partyManager;
            _expManager = expManager;
            _statusManager = statusManager;
            _assets = assets;
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task ExecuteCommand(GameClient client, string message)
        {
            var command = Regex.Replace(message.Trim(), @"\s+", " ").Split(' ');

            if (message.Contains("summon") && command[0].ToLower() != "summon")
            {
                command[2] = message.Split(' ')[2];
            }

            _logger.Information($"GM AccountID: {client.AccountId} Tamer: {client.Tamer.Name} used Command !{message}");

            switch (command[0].ToLower())
            {
                case "players":
                    if (command.Length == 1)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }
                    switch (command[1].ToLower())
                    {
                        case "count":
                        {
                            client.Send(new SystemMessagePacket($"Clients count: {client.Server.Clients.Count}"));
                            break;
                        }

                    }
                    break;
                
                case "hatch":
                {
                    var regex = @"^hatch";
                    var match = Regex.IsMatch(message, regex, RegexOptions.IgnoreCase);

                    if (!match)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !hatch (Type) (Name)"));
                        break;
                    }

                    if (command.Length < 3)
                    {
                        client.Send(new SystemMessagePacket("Invalid command.\nType !hatch (Type) (Name)"));
                        break;
                    }

                    if (!int.TryParse(command[1], out int digiId))
                    {
                        client.Send(new SystemMessagePacket("Invalid digimon Id.\nType numeric value."));
                        break;
                    }

                    var digiName = command[2];

                    /*if (digiId == 31001 || digiId == 31002 || digiId == 31003 || digiId == 31004)
                    {
                        client.Send(new SystemMessagePacket($"You cant hatch starter digimon, sorry :P"));
                        break;
                    }*/

                    var digiBase = _assets.DigimonBaseInfo.First(x => x.Type == digiId);

                    if (digiBase == null)
                    {
                        client.Send(new SystemMessagePacket($"Digimon Type {digiId} not found !!"));
                        _logger.Error($"Digimon Type {digiId} not found on DigimonBaseInfo !! [ Hatch Command ]");
                        break;
                    }

                    try
                    {
                        var digiEvo = _assets.EvolutionInfo.First(x => x.Type == digiId);

                        if (digiEvo == null)
                        {
                            client.Send(
                                new SystemMessagePacket(
                                    $"Digimon Type {digiId} not available,\nneed to be Rookie/Spirit !!"));
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Digimon Type {digiId} not available,\nneed to be Rookie/Spirit !!"));
                        break;
                    }

                    /*byte i = 0;
                    while (i < client.Tamer.DigimonSlots)
                    {
                        if (client.Tamer.Digimons.FirstOrDefault(x => x.Slot == i) == null)
                            break;

                        i++;
                    }*/

                    byte digimonSlot = (byte)Enumerable.Range(0, client.Tamer.DigimonSlots)
                        .FirstOrDefault(slot => client.Tamer.Digimons.FirstOrDefault(x => x.Slot == slot) == null);

                    var newDigimon = DigimonModel.Create(digiName, digiId, digiId, DigimonHatchGradeEnum.Perfect, 12500,
                        digimonSlot);

                    newDigimon.NewLocation(client.Tamer.Location.MapId, client.Tamer.Location.X,
                        client.Tamer.Location.Y);

                    newDigimon.SetBaseInfo(_statusManager.GetDigimonBaseInfo(newDigimon.BaseType));
                    newDigimon.SetBaseStatus(_statusManager.GetDigimonBaseStatus(newDigimon.BaseType, newDigimon.Level,
                        newDigimon.Size));

                    var digimonEvolutionInfo = _assets.EvolutionInfo.First(x => x.Type == newDigimon.BaseType);

                    newDigimon.AddEvolutions(digimonEvolutionInfo);

                    if (newDigimon.BaseInfo == null || newDigimon.BaseStatus == null || !newDigimon.Evolutions.Any())
                    {
                        _logger.Error($"Unknown digimon info for {newDigimon.BaseType}.");
                        client.Send(new SystemMessagePacket($"Unknown digimon info for {newDigimon.BaseType}."));
                        return;
                    }

                    newDigimon.SetTamer(client.Tamer);

                    client.Send(new HatchFinishPacket(newDigimon, (ushort)(client.Partner.GeneralHandler + 1000),
                        newDigimon.Slot));

                    var digimonInfo =
                        _mapper.Map<DigimonModel>(await _sender.Send(new CreateDigimonCommand(newDigimon)));
                    client.Tamer.AddDigimon(digimonInfo);
                    if (digimonInfo != null)
                    {
                        newDigimon.SetId(digimonInfo.Id);
                        var slot = -1;

                        foreach (var digimon in newDigimon.Evolutions)
                        {
                            slot++;

                            var evolution = digimonInfo.Evolutions[slot];

                            if (evolution != null)
                            {
                                digimon.SetId(evolution.Id);

                                var skillSlot = -1;

                                foreach (var skill in digimon.Skills)
                                {
                                    skillSlot++;

                                    var dtoSkill = evolution.Skills[skillSlot];

                                    skill.SetId(dtoSkill.Id);
                                }
                            }
                        }
                    }

                    client.Send(new NeonMessagePacket(NeonMessageTypeEnum.Scale, client.Tamer.Name, newDigimon.BaseType,
                        newDigimon.Size).Serialize());

                    // ------------------------------------------------------------------------------------------------------

                    var digimonBaseInfo = newDigimon.BaseInfo;
                    var digimonEvolutions = newDigimon.Evolutions;

                    //_logger.Information($"DigimonType: {newDigimon.BaseType} | DigimonInfo: {digimonEvolutionInfo?.Id.ToString()}");

                    var encyclopediaExists =
                        client.Tamer.Encyclopedia.Exists(x => x.DigimonEvolutionId == digimonEvolutionInfo?.Id);

                    // Check if encyclopedia exists
                    if (!encyclopediaExists && digimonEvolutionInfo != null)
                    {
                        var encyclopedia = CharacterEncyclopediaModel.Create(client.TamerId, digimonEvolutionInfo.Id,
                            newDigimon.Level, newDigimon.Size, 0, 0, 0, 0, 0, false, false);

                        digimonEvolutions?.ForEach(x =>
                        {
                            var evolutionLine = digimonEvolutionInfo.Lines.FirstOrDefault(y => y.Type == x.Type);
                            byte slotLevel = 0;

                            if (evolutionLine != null)
                            {
                                slotLevel = evolutionLine.SlotLevel;
                            }

                            var encyclopediaEvo =
                                CharacterEncyclopediaEvolutionsModel.Create(x.Type, slotLevel,
                                    Convert.ToBoolean(x.Unlocked));

                            _logger.Debug(
                                $"{encyclopediaEvo.Id}, {encyclopediaEvo.DigimonBaseType}, {encyclopediaEvo.SlotLevel}, {encyclopediaEvo.IsUnlocked}");

                            encyclopedia.Evolutions.Add(encyclopediaEvo);
                        });

                        var encyclopediaAdded =
                            await _sender.Send(new CreateCharacterEncyclopediaCommand(encyclopedia));

                        client.Tamer.Encyclopedia.Add(encyclopediaAdded);
                    }
                }
                    break;

                case "delete":
                {
                    var regex = @"^delete";
                    var match = Regex.IsMatch(message, regex, RegexOptions.IgnoreCase);

                    if (!match)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !delete (slot) (email)"));
                        break;
                    }

                    if (command.Length < 3)
                    {
                        client.Send(new SystemMessagePacket("Invalid command.\nType !delete (slot) (email)"));
                        break;
                    }

                    if (!byte.TryParse(command[1], out byte digiSlot))
                    {
                        client.Send(new SystemMessagePacket("Invalid Slot.\nType a valid Slot (1 to 4)"));
                        break;
                    }

                    if (digiSlot == 0)
                    {
                        client.Send(new SystemMessagePacket($"Digimon in slot 0 cant be deleted !!"));
                        break;
                    }

                    string validation = command[2].ToLower();

                    var digimon = client.Tamer.Digimons.FirstOrDefault(x => x.Slot == digiSlot);

                    if (digimon == null)
                    {
                        client.Send(new SystemMessagePacket($"Digimon not found on slot {digiSlot}"));
                        break;
                    }

                    var digimonId = digimon.Id;

                    var result = client.PartnerDeleteValidation(validation);

                    if (result > 0)
                    {
                        client.Tamer.RemoveDigimon(digiSlot);

                        client.Send(new PartnerDeletePacket(digiSlot));

                        await _sender.Send(new DeleteDigimonCommand(digimonId));

                        _logger.Verbose($"Tamer {client.Tamer.Name} deleted partner {digimonId}.");
                    }
                    else
                    {
                        client.Send(new PartnerDeletePacket(result));
                        _logger.Verbose(
                            $"Tamer {client.Tamer.Name} failed to deleted partner {digimonId} with invalid account information.");
                    }
                }
                    break;

                case "done":
                {
                    var regex = @"^done";
                    var match = Regex.IsMatch(message, regex, RegexOptions.IgnoreCase);

                    if (!match)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !done (slot)"));
                        break;
                    }

                    if (command.Length < 2)
                    {
                        client.Send(new SystemMessagePacket("Invalid command.\nType !delete (slot)"));
                        break;
                    }

                    if (!byte.TryParse(command[1], out byte digiSlot))
                    {
                        client.Send(new SystemMessagePacket("Invalid Slot.\nType a valid Slot (1 to 4)"));
                        break;
                    }

                    if (digiSlot == 0 || digiSlot > 4)
                    {
                        client.Send(new SystemMessagePacket($"Invalid Slot !!"));
                        break;
                    }

                    var digimon = client.Tamer.Digimons.FirstOrDefault(x => x.Slot == digiSlot);

                    if (digimon == null)
                    {
                        client.Send(new SystemMessagePacket($"Digimon not found on slot {digiSlot}"));
                        break;
                    }
                    else
                    {
                        var digimonId = digimon.Id;

                        if (digimon.BaseType == 31066 && digimon.Level >= 99)
                        {
                            var itemId = 66935; // 66935 impmon item

                            var newItem = new ItemModel();
                            newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == itemId));

                            if (newItem.ItemInfo == null)
                            {
                                _logger.Warning($"No item info found with ID {itemId} for tamer {client.TamerId}.");
                                client.Send(new SystemMessagePacket($"No item info found with ID {itemId}."));
                                break;
                            }

                            newItem.ItemId = itemId;
                            newItem.Amount = 1;

                            if (newItem.IsTemporary)
                                newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                            var itemClone = (ItemModel)newItem.Clone();

                            if (client.Tamer.Inventory.AddItem(newItem))
                            {
                                client.Send(new ReceiveItemPacket(newItem, InventoryTypeEnum.Inventory));
                                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                            }
                            else
                            {
                                client.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                                client.Send(new SystemMessagePacket($"Inventory full !!"));
                                break;
                            }

                            client.Tamer.RemoveDigimon(digiSlot);

                            client.Send(new PartnerDeletePacket(digiSlot));

                            await _sender.Send(new DeleteDigimonCommand(digimonId));
                        }
                        else if (digimon.BaseType == 31023 && digimon.Level >= 99) // Sleipmon Type
                        {
                            var itemId = 66936; // 66936 slepymon item

                            var newItem = new ItemModel();
                            newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == itemId));

                            if (newItem.ItemInfo == null)
                            {
                                _logger.Warning($"No item info found with ID {itemId} for tamer {client.TamerId}.");
                                client.Send(new SystemMessagePacket($"No item info found with ID {itemId}."));
                                break;
                            }

                            newItem.ItemId = itemId;
                            newItem.Amount = 1;

                            if (newItem.IsTemporary)
                                newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                            var itemClone = (ItemModel)newItem.Clone();

                            if (client.Tamer.Inventory.AddItem(newItem))
                            {
                                client.Send(new ReceiveItemPacket(newItem, InventoryTypeEnum.Inventory));
                                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                            }
                            else
                            {
                                client.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                                client.Send(new SystemMessagePacket($"Inventory full !!"));
                                break;
                            }

                            client.Tamer.RemoveDigimon(digiSlot);

                            client.Send(new PartnerDeletePacket(digiSlot));

                            await _sender.Send(new DeleteDigimonCommand(digimonId));
                        }
                        else if (digimon.BaseType == 31022 && digimon.Level >= 99) // Dynasmon Type
                        {
                            var itemId = 66937; // 66937 dynasmon item

                            var newItem = new ItemModel();
                            newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == itemId));

                            if (newItem.ItemInfo == null)
                            {
                                _logger.Warning($"No item info found with ID {itemId} for tamer {client.TamerId}.");
                                client.Send(new SystemMessagePacket($"No item info found with ID {itemId}."));
                                break;
                            }

                            newItem.ItemId = itemId;
                            newItem.Amount = 1;

                            if (newItem.IsTemporary)
                                newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                            var itemClone = (ItemModel)newItem.Clone();

                            if (client.Tamer.Inventory.AddItem(newItem))
                            {
                                client.Send(new ReceiveItemPacket(newItem, InventoryTypeEnum.Inventory));
                                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                            }
                            else
                            {
                                client.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                                client.Send(new SystemMessagePacket($"Inventory full !!"));
                                break;
                            }

                            client.Tamer.RemoveDigimon(digiSlot);

                            client.Send(new PartnerDeletePacket(digiSlot));

                            await _sender.Send(new DeleteDigimonCommand(digimonId));
                        }
                        else
                        {
                            client.Send(new SystemMessagePacket("Wrong digimon type or level less than 99!!"));
                            break;
                        }
                    }
                }
                    break;

                case "tamer":
                {
                    if (command.Length == 1)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    switch (command[1].ToLower())
                    {
                        case "size":
                        {
                            var regex = @"(tamer\ssize\s\d){1}";
                            var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                            if (!match.Success)
                            {
                                client.Send(new SystemMessagePacket(
                                    $"Unknown command. Check the available commands on the Admin Portal."));
                                break;
                            }

                            if (short.TryParse(command[2], out var value))
                            {
                                client.Tamer.SetSize(value);

                                if (client.DungeonMap)
                                {
                                    _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new UpdateSizePacket(client.Tamer.GeneralHandler, client.Tamer.Size)
                                            .Serialize());
                                }
                                else
                                {
                                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new UpdateSizePacket(client.Tamer.GeneralHandler, client.Tamer.Size)
                                            .Serialize());
                                }

                                await _sender.Send(new UpdateCharacterSizeCommand(client.TamerId, value));
                            }
                            else
                            {
                                client.Send(
                                    new SystemMessagePacket(
                                        $"Invalid value. Max possible amount is {short.MaxValue}."));
                            }
                        }
                            break;

                        case "exp":
                        {
                            //TODO: refazer
                            var regex = @"(tamer\sexp\sadd\s\d){1}|(tamer\sexp\sremove\s\d){1}|(tamer\sexp\smax){1}";
                            var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                            if (!match.Success)
                            {
                                client.Send(new SystemMessagePacket("Correct usage is \"!tamer exp add value\" or " +
                                                                    "\"!tamer exp remove value\"" +
                                                                    "\"!tamer exp max\".")
                                    .Serialize());

                                break;
                            }

                            switch (command[2])
                            {
                                case "max":
                                {
                                    if (client.Tamer.Level >= (int)GeneralSizeEnum.TamerLevelMax)
                                    {
                                        client.Send(new SystemMessagePacket($"Tamer already at max level."));
                                        break;
                                    }

                                    var result = _expManager.ReceiveMaxTamerExperience(client.Tamer);

                                    if (result.Success)
                                    {
                                        client.Send(
                                            new ReceiveExpPacket(
                                                0,
                                                0,
                                                client.Tamer.CurrentExperience,
                                                client.Tamer.Partner.GeneralHandler,
                                                0,
                                                0,
                                                client.Tamer.Partner.CurrentExperience,
                                                0
                                            )
                                        );
                                    }
                                    else
                                    {
                                        client.Send(new SystemMessagePacket(
                                            $"No proper configuration for tamer {client.Tamer.Model} leveling."));
                                        return;
                                    }

                                    if (result.LevelGain > 0)
                                    {
                                        client.Tamer.SetLevelStatus(
                                            _statusManager.GetTamerLevelStatus(
                                                client.Tamer.Model,
                                                client.Tamer.Level
                                            )
                                        );

                                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new LevelUpPacket(client.Tamer.GeneralHandler, client.Tamer.Level)
                                                .Serialize());

                                        client.Tamer.FullHeal();

                                        client.Send(new UpdateStatusPacket(client.Tamer));

                                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new UpdateMovementSpeedPacket(client.Tamer).Serialize());
                                    }

                                    if (result.Success)
                                    {
                                        await _sender.Send(new UpdateCharacterExperienceCommand(client.TamerId,
                                            client.Tamer.CurrentExperience, client.Tamer.Level));
                                    }
                                }
                                    break;

                                case "add":
                                {
                                    if (client.Tamer.Level >= (int)GeneralSizeEnum.TamerLevelMax)
                                    {
                                        client.Send(new SystemMessagePacket($"Tamer already at max level."));
                                        break;
                                    }

                                    var value = Convert.ToInt64(command[3]);

                                    var result = _expManager.ReceiveTamerExperience(value, client.Tamer);

                                    if (result.Success)
                                    {
                                        client.Send(
                                            new ReceiveExpPacket(
                                                value,
                                                0,
                                                client.Tamer.CurrentExperience,
                                                client.Tamer.Partner.GeneralHandler,
                                                0,
                                                0,
                                                client.Tamer.Partner.CurrentExperience,
                                                0
                                            )
                                        );
                                    }
                                    else
                                    {
                                        client.Send(new SystemMessagePacket(
                                            $"No proper configuration for tamer {client.Tamer.Model} leveling."));
                                        return;
                                    }

                                    if (result.LevelGain > 0)
                                    {
                                        client.Tamer.SetLevelStatus(
                                            _statusManager.GetTamerLevelStatus(
                                                client.Tamer.Model,
                                                client.Tamer.Level
                                            )
                                        );

                                        _mapServer.BroadcastForTamerViewsAndSelf(
                                            client.TamerId,
                                            new LevelUpPacket(
                                                client.Tamer.GeneralHandler,
                                                client.Tamer.Level).Serialize());

                                        client.Tamer.FullHeal();

                                        client.Send(new UpdateStatusPacket(client.Tamer));
                                    }

                                    if (result.Success)
                                        await _sender.Send(new UpdateCharacterExperienceCommand(client.TamerId,
                                            client.Tamer.CurrentExperience, client.Tamer.Level));
                                }
                                    break;

                                case "remove":
                                {
                                    var value = Convert.ToInt64(command[3]);

                                    var tamerInfos = _assets.TamerLevelInfo
                                        .Where(x => x.Type == client.Tamer.Model)
                                        .ToList();

                                    if (tamerInfos == null || !tamerInfos.Any() ||
                                        tamerInfos.Count != (int)GeneralSizeEnum.TamerLevelMax)
                                    {
                                        _logger.Warning($"Incomplete level config for tamer {client.Tamer.Model}.");

                                        client.Send(new SystemMessagePacket
                                            ($"No proper configuration for tamer {client.Tamer.Model} leveling."));
                                        break;
                                    }

                                    //TODO: ajeitar
                                    client.Tamer.LooseExp(value);

                                    client.Send(new ReceiveExpPacket(
                                        value * -1,
                                        0,
                                        client.Tamer.CurrentExperience,
                                        client.Tamer.Partner.GeneralHandler,
                                        0,
                                        0,
                                        client.Tamer.Partner.CurrentExperience,
                                        0
                                    ));

                                    await _sender.Send(new UpdateCharacterExperienceCommand(client.TamerId,
                                        client.Tamer.CurrentExperience, client.Tamer.Level));
                                }
                                    break;

                                default:
                                {
                                    client.Send(new SystemMessagePacket(
                                        "Correct usage is \"!tamer exp add {value}\" or " +
                                        "\"!tamer exp max\"."));
                                }
                                    break;
                            }
                        }
                            break;

                        case "summon":
                        {
                            var tamerName = command[2];
                            var TargetSummon = _mapServer.FindClientByTamerName(tamerName);

                            if (TargetSummon == null) TargetSummon = _dungeonServer.FindClientByTamerName(tamerName);

                            if (TargetSummon == null)
                            {
                                client.Send(new SystemMessagePacket($"Tamer {tamerName} is not online!"));
                                return;
                            }

                            _logger.Information(
                                $"Tamer: {client.Tamer.Name} is summoning Tamer: {TargetSummon.Tamer.Name}");
                            var mapId = client.Tamer.Location.MapId;
                            var destination = client.Tamer.Location;

                            if (TargetSummon.DungeonMap)
                                _dungeonServer.RemoveClient(TargetSummon);
                            else
                                _mapServer.RemoveClient(TargetSummon);

                            TargetSummon.Tamer.NewLocation(mapId, destination.X, destination.Y);
                            await _sender.Send(new UpdateCharacterLocationCommand(TargetSummon.Tamer.Location));

                            TargetSummon.Tamer.Partner.NewLocation(mapId, destination.X, destination.Y);
                            await _sender.Send(new UpdateDigimonLocationCommand(TargetSummon.Tamer.Partner.Location));

                            TargetSummon.Tamer.UpdateState(CharacterStateEnum.Loading);
                            await _sender.Send(new UpdateCharacterStateCommand(TargetSummon.TamerId,
                                CharacterStateEnum.Loading));

                            TargetSummon.SetGameQuit(false);

                            TargetSummon.Send(new MapSwapPacket(_configuration[GamerServerPublic],
                                _configuration[GameServerPort],
                                client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y));

                            var party = _partyManager.FindParty(TargetSummon.TamerId);

                            if (party != null)
                            {
                                party.UpdateMember(party[TargetSummon.TamerId], TargetSummon.Tamer);

                                _mapServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                                    new PartyMemberWarpGatePacket(party[TargetSummon.TamerId], client.Tamer)
                                        .Serialize());

                                _dungeonServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                                    new PartyMemberWarpGatePacket(party[TargetSummon.TamerId], client.Tamer)
                                        .Serialize());
                            }

                            client.Send(new SystemMessagePacket($"You summoned Tamer: {TargetSummon.Tamer.Name}"));
                            TargetSummon.Send(
                                new SystemMessagePacket($"You have been summoned by Tamer: {client.Tamer.Name}"));
                        }
                            break;

                        default:
                        {
                            client.Send(new SystemMessagePacket("Under development."));
                        }
                            break;
                    }
                }
                    break;

                case "digimon":
                {
                    if (command.Length == 1)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    switch (command[1].ToLower())
                    {
                        case "transcend":
                        {
                            var regex = @"(digimon\stranscend){1}";
                            var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                            if (!match.Success)
                            {
                                client.Send(new SystemMessagePacket(
                                    $"Unknown command. Check the available commands on the Admin Portal."));
                                break;
                            }

                            client.Partner.Transcend();
                            client.Partner.SetSize(14000);

                            client.Partner.SetBaseStatus(
                                _statusManager.GetDigimonBaseStatus(
                                    client.Partner.CurrentType,
                                    client.Partner.Level,
                                    client.Partner.Size
                                )
                            );

                            await _sender.Send(new UpdateDigimonSizeCommand(client.Partner.Id, client.Partner.Size));
                            await _sender.Send(new UpdateDigimonGradeCommand(client.Partner.Id,
                                client.Partner.HatchGrade));

                            client.Tamer.UpdateState(CharacterStateEnum.Loading);
                            await _sender.Send(new UpdateCharacterStateCommand(client.TamerId,
                                CharacterStateEnum.Loading));

                            _mapServer.RemoveClient(client);

                            client.SetGameQuit(false);
                            client.Tamer.UpdateSlots();

                            client.Send(new MapSwapPacket(_configuration[GamerServerPublic],
                                _configuration[GameServerPort],
                                client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y));
                        }
                            break;

                        case "size":
                        {
                            var regex = @"(digimon\ssize\s\d){1}";
                            var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                            if (!match.Success)
                            {
                                client.Send(new SystemMessagePacket(
                                    $"Unknown command. Check the available commands on the Admin Portal."));
                                break;
                            }

                            if (short.TryParse(command[2], out var value))
                            {
                                client.Partner.SetSize(value);
                                client.Partner.SetBaseStatus(
                                    _statusManager.GetDigimonBaseStatus(
                                        client.Partner.CurrentType,
                                        client.Partner.Level,
                                        client.Partner.Size
                                    )
                                );

                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new UpdateSizePacket(client.Partner.GeneralHandler, client.Partner.Size)
                                        .Serialize());
                                client.Send(new UpdateStatusPacket(client.Tamer));
                                await _sender.Send(new UpdateDigimonSizeCommand(client.Partner.Id, value));
                            }
                            else
                            {
                                client.Send(
                                    new SystemMessagePacket(
                                        $"Invalid value. Max possible amount is {short.MaxValue}."));
                            }
                        }
                            break;

                        case "exp":
                        {
                            var regex =
                                @"(digimon\sexp\sadd\s\d){1}|(digimon\sexp\sremove\s\d){1}|(digimon\sexp\smax){1}";
                            var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                            if (!match.Success)
                            {
                                client.Send(new SystemMessagePacket("Correct usage is \"!digimon exp add value\" or " +
                                                                    "\"!digimon exp remove value\" or " +
                                                                    "\"!digimon exp max\".")
                                    .Serialize());

                                break;
                            }

                            switch (command[2])
                            {
                                case "max":
                                {
                                    if (client.Partner.Level >= (int)GeneralSizeEnum.DigimonLevelMax)
                                    {
                                        client.Send(new SystemMessagePacket(
                                            $"Partner already at max level {(int)GeneralSizeEnum.DigimonLevelMax}..."));
                                        break;
                                    }

                                    var result = _expManager.ReceiveMaxDigimonExperience(client.Partner);

                                    var mapConfig =
                                        await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

                                    if (result.Success)
                                    {
                                        client.Send(new ReceiveExpPacket(0, 0, client.Tamer.CurrentExperience,
                                            client.Tamer.Partner.GeneralHandler, 0, 0,
                                            client.Tamer.Partner.CurrentExperience, 0));

                                        if (result.LevelGain > 0)
                                        {
                                            client.Partner.SetBaseStatus(
                                                _statusManager.GetDigimonBaseStatus(client.Partner.CurrentType,
                                                    client.Partner.Level, client.Partner.Size));

                                            switch (mapConfig.Type)
                                            {
                                                case MapTypeEnum.Dungeon:
                                                    _dungeonServer.BroadcastForTamerViewsAndSelf(client,
                                                        new LevelUpPacket(client.Tamer.Partner.GeneralHandler,
                                                            client.Tamer.Partner.Level).Serialize());
                                                    break;
                                                case MapTypeEnum.Event:
                                                    _eventServer.BroadcastForTamerViewsAndSelf(client,
                                                        new LevelUpPacket(client.Tamer.Partner.GeneralHandler,
                                                            client.Tamer.Partner.Level).Serialize());
                                                    break;
                                                case MapTypeEnum.Pvp:
                                                    _pvpServer.BroadcastForTamerViewsAndSelf(client,
                                                        new LevelUpPacket(client.Tamer.Partner.GeneralHandler,
                                                            client.Tamer.Partner.Level).Serialize());
                                                    break;
                                                default:
                                                    _mapServer.BroadcastForTamerViewsAndSelf(client,
                                                        new LevelUpPacket(client.Tamer.Partner.GeneralHandler,
                                                            client.Tamer.Partner.Level).Serialize());
                                                    break;
                                            }

                                            client.Partner.FullHeal();

                                            client.Send(new UpdateStatusPacket(client.Tamer));

                                            switch (mapConfig.Type)
                                            {
                                                case MapTypeEnum.Dungeon:
                                                    _dungeonServer.BroadcastForTamerViewsAndSelf(client,
                                                        new UpdateMovementSpeedPacket(client.Tamer).Serialize());
                                                    break;
                                                case MapTypeEnum.Event:
                                                    _eventServer.BroadcastForTamerViewsAndSelf(client,
                                                        new UpdateMovementSpeedPacket(client.Tamer).Serialize());
                                                    break;
                                                case MapTypeEnum.Pvp:
                                                    _pvpServer.BroadcastForTamerViewsAndSelf(client,
                                                        new UpdateMovementSpeedPacket(client.Tamer).Serialize());
                                                    break;
                                                default:
                                                    _mapServer.BroadcastForTamerViewsAndSelf(client,
                                                        new UpdateMovementSpeedPacket(client.Tamer).Serialize());
                                                    break;
                                            }
                                        }

                                        await _sender.Send(new UpdateDigimonExperienceCommand(client.Partner));
                                    }
                                    else
                                    {
                                        client.Send(new SystemMessagePacket(
                                            $"No proper configuration for digimon {client.Partner.Model} leveling."));
                                        return;
                                    }
                                }
                                    break;

                                case "add":
                                {
                                    if (client.Partner.Level == (int)GeneralSizeEnum.DigimonLevelMax)
                                    {
                                        client.Send(new SystemMessagePacket($"Partner already at max level."));
                                        break;
                                    }

                                    var value = Convert.ToInt64(command[3]);

                                    var result = _expManager.ReceiveDigimonExperience(value, client.Partner);

                                    if (result.Success)
                                    {
                                        client.Send(
                                            new ReceiveExpPacket(
                                                0,
                                                0,
                                                client.Tamer.CurrentExperience,
                                                client.Tamer.Partner.GeneralHandler,
                                                value,
                                                0,
                                                client.Tamer.Partner.CurrentExperience,
                                                0
                                            )
                                        );
                                    }
                                    else
                                    {
                                        client.Send(new SystemMessagePacket(
                                            $"No proper configuration for digimon {client.Partner.Model} leveling."));
                                        return;
                                    }

                                    if (result.LevelGain > 0)
                                    {
                                        client.Partner.SetBaseStatus(
                                            _statusManager.GetDigimonBaseStatus(
                                                client.Partner.CurrentType,
                                                client.Partner.Level,
                                                client.Partner.Size
                                            )
                                        );

                                        _mapServer.BroadcastForTamerViewsAndSelf(
                                            client.TamerId,
                                            new LevelUpPacket(
                                                client.Tamer.Partner.GeneralHandler,
                                                client.Tamer.Partner.Level
                                            ).Serialize()
                                        );

                                        client.Partner.FullHeal();

                                        client.Send(new UpdateStatusPacket(client.Tamer));
                                    }

                                    if (result.Success)
                                        await _sender.Send(new UpdateDigimonExperienceCommand(client.Partner));
                                }
                                    break;

                                case "remove":
                                {
                                    var value = Convert.ToInt64(command[3]);

                                    var digimonInfos = _assets.DigimonLevelInfo
                                        .Where(x => x.Type == client.Tamer.Partner.BaseType)
                                        .ToList();

                                    if (digimonInfos == null || !digimonInfos.Any() ||
                                        digimonInfos.Count != (int)GeneralSizeEnum.DigimonLevelMax)
                                    {
                                        _logger.Warning(
                                            $"Incomplete level config for digimon {client.Tamer.Partner.BaseType}.");

                                        client.Send(new SystemMessagePacket
                                            ($"No proper configuration for digimon {client.Tamer.Partner.BaseType} leveling."));
                                        break;
                                    }

                                    //TODO: ajeitar
                                    var partnerInitialLevel = client.Partner.Level;

                                    client.Tamer.LooseExp(value);

                                    client.Send(new ReceiveExpPacket(
                                        0,
                                        0,
                                        client.Tamer.CurrentExperience,
                                        client.Tamer.Partner.GeneralHandler,
                                        value * -1,
                                        0,
                                        client.Tamer.Partner.CurrentExperience,
                                        0
                                    ));

                                    if (partnerInitialLevel != client.Partner.Level)
                                        client.Send(new LevelUpPacket(client.Partner.GeneralHandler,
                                            client.Partner.Level));

                                    await _sender.Send(new UpdateDigimonExperienceCommand(client.Partner));
                                }
                                    break;

                                default:
                                {
                                    client.Send(new SystemMessagePacket(
                                        "Correct usage is \"!digimon exp add value\" or " +
                                        "\"!digimon exp max\"."));
                                }
                                    break;
                            }
                        }
                            break;

                        default:
                        {
                            client.Send(
                                new SystemMessagePacket(
                                    "Unknown command. Check the available commands at the admin portal."));
                        }
                            break;
                    }
                }
                    break;

                case "currency":
                {
                    var regex = @"(currency\sbits\s\d){1}|(currency\spremium\s\d){1}|(currency\ssilk\s\d){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    switch (command[1].ToLower())
                    {
                        case "bits":
                        {
                            var value = long.Parse(command[2]);
                            client.Tamer.Inventory.AddBits(value);

                            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory)
                                .Serialize());

                            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory.Id,
                                client.Tamer.Inventory.Bits));
                        }
                            break;

                        case "premium":
                        {
                            var value = int.Parse(command[2]);
                            client.AddPremium(value);

                            await _sender.Send(new UpdatePremiumAndSilkCommand(client.Premium, client.Silk,
                                client.AccountId));
                        }
                            break;

                        case "silk":
                        {
                            var value = int.Parse(command[2]);
                            client.AddSilk(value);

                            await _sender.Send(new UpdatePremiumAndSilkCommand(client.Premium, client.Silk,
                                client.AccountId));
                        }
                            break;
                    }
                }
                    break;

                case "reload":
                {
                    var regex = @"(reload$){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

                    _logger.Debug($"Updating tamer state...");
                    client.Tamer.UpdateState(CharacterStateEnum.Loading);
                    await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

                    switch (mapConfig.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonServer.RemoveClient(client);
                            break;
                        case MapTypeEnum.Event:
                            _eventServer.RemoveClient(client);
                            break;
                        case MapTypeEnum.Pvp:
                            _pvpServer.RemoveClient(client);
                            break;
                        default:
                            _mapServer.RemoveClient(client);
                            break;
                    }

                    client.SetGameQuit(false);
                    client.Tamer.UpdateSlots();

                    client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                        client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y));
                }
                    break;

                case "dc":
                {
                    var regex = @"^dc\s[\w\s]+$";
                    var match = Regex.Match(message, regex, RegexOptions.None);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !dc TamerName"));
                        break;
                    }

                    string[] comando = message.Split(' ');
                    var TamerName = comando[1];

                    var targetClient = _mapServer.FindClientByTamerName(TamerName);
                    var targetClientD = _dungeonServer.FindClientByTamerName(TamerName);

                    if (targetClient == null && targetClientD == null)
                    {
                        client.Send(new SystemMessagePacket($"Player {TamerName} not Online!"));
                        break;
                    }

                    if (targetClient == null) targetClient = targetClientD;

                    if (client.Tamer.Name == TamerName)
                    {
                        client.Send(new SystemMessagePacket($"You are a {TamerName}!"));
                        break;
                    }

                    targetClient.Send(new SystemMessagePacket($"Voce foi kickado pela staff!"));
                    targetClient.Disconnect();
                }
                    break;

                case "ban":
                {
                    if (command.Length == 1)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    var otherText = string.Join(" ", message.Split(' ').Skip(1));
                    string targetName = command[1].ToLower();
                    string Time = command[2];
                    string banReason = string.Join(" ", otherText.Split(' ').Skip(2));

                    if (string.IsNullOrEmpty(banReason) || string.IsNullOrEmpty(Time))
                    {
                        client.Send(
                            new SystemMessagePacket($"Incorret command, use \"!ban TamerName Horas BanReason\"."));

                        return;
                    }

                    if (!string.IsNullOrEmpty(targetName))
                    {
                        var TargetBan = await _sender.Send(new CharacterByNameQuery(targetName));
                        if (TargetBan == null)
                        {
                            _logger.Warning($"Character not found with name {targetName}.");
                            client.Send(new SystemMessagePacket($"Character not found with name {targetName}."));
                            return;
                        }

                        var TargetBanId = await _sender.Send(new AccountByIdQuery(TargetBan.AccountId));

                        if (TargetBan == null)
                        {
                            client.Send(new SystemMessagePacket($"User not found with name {targetName}."));
                            return;
                        }

                        try
                        {
                            var banProcessor = SingletonResolver.GetService<BanForCheating>();
                            var accountBanned = banProcessor.BanAccountWithMessage(TargetBan.AccountId, TargetBan.Name,
                                AccountBlockEnum.Permanent, banReason);
                            if (accountBanned != null)
                            {
                                var banMessage = "A Tamer has been banned. We're keeping our community clean!";
                                client.SendToAll(new ChatMessagePacket(banMessage, ChatTypeEnum.Megaphone,
                                    "[System]", 52, 120).Serialize());
                                /*_mapServer.BroadcastGlobal(new ChatMessagePacket(banMessage, ChatTypeEnum.Megaphone,
                                    "[System]", 52, 120).Serialize());
                                _dungeonServer.BroadcastGlobal(new ChatMessagePacket(banMessage,
                                    ChatTypeEnum.Megaphone, "[System]", 52, 120).Serialize());*/

                                var TargetTamer = _mapServer.FindClientByTamerId(TargetBan.Id);
                                if (TargetTamer != null)
                                {
                                    _logger.Information(
                                        $"Found client {TargetTamer.ClientAddress} Tamer ID: {TargetTamer.TamerId}");

                                    TimeSpan timeRemaining = DateTime.MaxValue - DateTime.Now;

                                    uint secondsRemaining = (uint)timeRemaining.TotalSeconds;

                                    TargetTamer.Send(new BanUserPacket(secondsRemaining, banReason));
                                }
                                else
                                {
                                    _logger.Information($"Banned client is not found");
                                }

                                break;
                            }
                            else
                            {
                                client.Send(new SystemMessagePacket(
                                    $"SERVER: There was an error trying to ban the user {targetName}, Check and try again."));
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(
                                $"Unexpected error on friend create request. Ex.: {ex.Message}. Stack: {ex.StackTrace}");
                        }
                    }
                }
                    break;

                case "loadmonster":
                {
                    try
                    {
                        if (client.Tamer.AccountId != 911)
                        {
                            break;
                        }

                        _logger.Information($"Received command: {message}");

                        var regex = @"^loadmonster\s\d+$"; // Match only "!loadmonster <SummonId>"
                        var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                        if (!match.Success)
                        {
                            client.Send(new SystemMessagePacket($"Unknown command.\nType !loadmonster SummonId"));
                            break;
                        }

                        // Split command into parts
                        var commandParts = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        // Ensure at least 2 parameters (!su SummonId)
                        if (commandParts.Length < 2 || !int.TryParse(commandParts[1], out var summonId))
                        {
                            client.Send(new SystemMessagePacket($"Invalid Summon ID !!"));
                            break;
                        }

                        _logger.Information($"Parsed Summon ID: {summonId}");

                        // Find summon info based on Summon ID
                        var summonInfo = _assets.SummonInfo.FirstOrDefault(x => x.Id == summonId);
                        if (summonInfo == null)
                        {
                            client.Send(new SystemMessagePacket($"Invalid Summon ID !!"));
                            break;
                        }

                        foreach (var mobToAdd in summonInfo?.SummonedMobs ?? Enumerable.Empty<SummonMobModel>())
                        {
                            var mob = (SummonMobModel)mobToAdd.Clone();
                            var matchingMaps = _mapServer.Maps.Where(x => x.MapId == mob.Location.MapId).ToList();

                            foreach (var map in matchingMaps)
                            {
                                if (map.SummonMobs.Any(existingMob => existingMob.Id == mob.Id))
                                {
                                    continue;
                                }

                                mob.TamersViewing.Clear();
                                mob.Reset();
                                mob.SetRespawn();
                                mob.SetId(mob.Id);
                                mob.SetLocation(mob.Location.MapId, mob.Location.X, mob.Location.Y);
                                mob.SetDuration();
                                _mapServer.AddSummonMobs(mob.Location.MapId,mob);

                                _logger.Information($"Mob {mob.Type} : {mob.Name} spawned from Summon ID {summonId}!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error spawning mob: {ex.Message}");
                    }
                }
                    break;
                
                case "item":
                {
                    var regex = @"(item\s\d{1,7}\s\d{1,4}$){1}|(item\s\d{1,7}$){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    var itemId = int.Parse(command[1].ToLower());

                    var newItem = new ItemModel();
                    newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == itemId));

                    if (newItem.ItemInfo == null)
                    {
                        _logger.Warning($"No item info found with ID {itemId} for tamer {client.TamerId}.");
                        client.Send(new SystemMessagePacket($"No item info found with ID {itemId}."));
                        break;
                    }

                    newItem.ItemId = itemId;
                    newItem.Amount = command.Length == 2 ? 1 : int.Parse(command[2]);

                    if (newItem.IsTemporary)
                        newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                    var itemClone = (ItemModel)newItem.Clone();
                    if (client.Tamer.Inventory.AddItem(newItem))
                    {
                        client.Send(new ReceiveItemPacket(newItem, InventoryTypeEnum.Inventory));
                        await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                    }
                    else
                    {
                        client.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                    }
                }
                    break;

                case "gfstorage":
                {
                    var regex = @"^(gfstorage\s(add\s\d{1,7}\s\d{1,4}|clear))$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command.\nType: gfstorage add itemId Amount or gfstorage clear"));
                        break;
                    }

                    if (command[1].ToLower() == "clear")
                    {
                        client.Tamer.GiftWarehouse.Clear();

                        client.Send(new SystemMessagePacket($" GiftStorage slots cleaned."));
                        client.Send(new LoadGiftStoragePacket(client.Tamer.GiftWarehouse));

                        await _sender.Send(new UpdateItemsCommand(client.Tamer.GiftWarehouse));
                    }
                    else if (command[1].ToLower() == "add")
                    {
                        if (!int.TryParse(command[2], out var itemId))
                        {
                            client.Send(new SystemMessagePacket($"Invalid ItemID format."));
                            break;
                        }

                        var amount = command.Length == 3
                            ? 1
                            : (int.TryParse(command[3], out var parsedAmount) ? parsedAmount : 1);

                        var newItem = new ItemModel();
                        newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == itemId));

                        if (newItem.ItemInfo == null)
                        {
                            _logger.Warning($"No item info found with ID {itemId} for tamer {client.TamerId}.");
                            client.Send(new SystemMessagePacket($"No item info found with ID {itemId}."));
                            break;
                        }

                        newItem.ItemId = itemId;
                        newItem.Amount = amount;

                        if (newItem.IsTemporary)
                            newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                        newItem.EndDate = DateTime.Now.AddDays(7);

                        if (client.Tamer.GiftWarehouse.AddItemGiftStorage(newItem))
                        {
                            await _sender.Send(new UpdateItemsCommand(client.Tamer.GiftWarehouse));
                            client.Send(new SystemMessagePacket($"Added x{amount} item {itemId} to GiftStorage."));
                        }
                        else
                        {
                            client.Send(
                                new SystemMessagePacket(
                                    $"Could not add item {itemId} to GiftStorage. Slots may be full."));
                        }
                    }
                }
                    break;

                case "cashstorage":
                {
                    var regex = @"^(cashstorage\s(add\s\d{1,7}\s\d{1,4}|clear))$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket(
                            $"Unknown command.\nType: cashstorage add itemId Amount or cashstorage clear"));
                        break;
                    }

                    if (command[1].ToLower() == "clear")
                    {
                        client.Tamer.AccountCashWarehouse.Clear();

                        client.Send(new SystemMessagePacket($" CashStorage slots cleaned."));
                        client.Send(new LoadAccountWarehousePacket(client.Tamer.AccountCashWarehouse));

                        await _sender.Send(new UpdateItemsCommand(client.Tamer.AccountCashWarehouse));
                    }
                    else if (command[1].ToLower() == "add")
                    {
                        if (!int.TryParse(command[2], out var itemId))
                        {
                            client.Send(new SystemMessagePacket($"Invalid ItemID format."));
                            break;
                        }

                        var amount = command.Length == 3
                            ? 1
                            : (int.TryParse(command[3], out var parsedAmount) ? parsedAmount : 1);

                        var newItem = new ItemModel();
                        newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == itemId));

                        if (newItem.ItemInfo == null)
                        {
                            _logger.Warning($"No item info found with ID {itemId} for tamer {client.TamerId}.");
                            client.Send(new SystemMessagePacket($"No item info found with ID {itemId}."));
                            break;
                        }

                        newItem.ItemId = itemId;
                        newItem.Amount = amount;

                        if (newItem.IsTemporary)
                            newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                        var itemClone = (ItemModel)newItem.Clone();

                        if (client.Tamer.AccountCashWarehouse.AddItemGiftStorage(newItem))
                        {
                            await _sender.Send(new UpdateItemsCommand(client.Tamer.AccountCashWarehouse));
                            client.Send(new SystemMessagePacket($"Added item {itemId} x{amount} to CashStorage."));
                        }
                        else
                        {
                            client.Send(
                                new SystemMessagePacket(
                                    $"Could not add item {itemId} to CashStorage. Slots may be full."));
                        }
                    }
                }
                    break;

                case "hide":
                {
                    var regex = @"(hide$){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    if (client.Tamer.Hidden)
                    {
                        client.Send(new SystemMessagePacket($"You are already in hide mode."));
                    }
                    else
                    {
                        client.Tamer.SetHidden(true);
                        client.Send(new SystemMessagePacket($"View state has been set to hide mode."));
                    }
                }
                    break;

                case "show":
                {
                    var regex = @"(show$){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    if (client.Tamer.Hidden)
                    {
                        client.Tamer.SetHidden(false);
                        client.Send(new SystemMessagePacket($"View state has been set to show mode."));
                    }
                    else
                    {
                        client.Send(new SystemMessagePacket($"You are already in show mode."));
                    }
                }
                    break;

                case "inv":
                {
                    var regex = @"^(inv\s+(add\s+\d{1,3}|clear))$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !inv add or !inv clear"));
                        break;
                    }

                    if (command[1].ToLower() == "add")
                    {
                        if (byte.TryParse(command[2], out byte targetSize) && targetSize > 0)
                        {
                            var newSize = client.Tamer.Inventory.AddSlots(targetSize);

                            client.Send(new SystemMessagePacket($"Inventory slots updated to {newSize}."));
                            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

                            var newSlots = client.Tamer.Inventory.Items.Where(x => x.ItemList == null).ToList();
                            await _sender.Send(new AddInventorySlotsCommand(newSlots));
                            newSlots.ForEach(newSlot =>
                            {
                                newSlot.ItemList = client.Tamer.Inventory.Items.First(x => x.ItemList != null)
                                    .ItemList;
                            });
                            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                        }
                        else
                        {
                            client.Send(new SystemMessagePacket(
                                $"Invalid command parameters. Check the available commands on the Admin Portal."));
                            break;
                        }
                    }
                    else if (command[1].ToLower() == "clear")
                    {
                        client.Tamer.Inventory.Clear();

                        client.Send(new SystemMessagePacket($"Inventory slots cleaned."));
                        client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

                        await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                    }
                }
                    break;

                case "storage":
                {
                    var regex = @"^(storage\s+(add\s+\d{1,3}|clear))$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !storage add or !storage clear"));
                        break;
                    }

                    if (command[1].ToLower() == "add")
                    {
                        if (byte.TryParse(command[2], out byte targetSize) && targetSize > 0)
                        {
                            var newSize = client.Tamer.Warehouse.AddSlots(targetSize);

                            client.Send(new SystemMessagePacket($"Inventory slots updated to {newSize}."));
                            client.Send(new LoadInventoryPacket(client.Tamer.Warehouse, InventoryTypeEnum.Warehouse));

                            var newSlots = client.Tamer.Warehouse.Items.Where(x => x.ItemList == null).ToList();
                            await _sender.Send(new AddInventorySlotsCommand(newSlots));
                            newSlots.ForEach(newSlot =>
                            {
                                newSlot.ItemList = client.Tamer.Warehouse.Items.First(x => x.ItemList != null)
                                    .ItemList;
                            });
                            await _sender.Send(new UpdateItemsCommand(client.Tamer.Warehouse));
                        }
                        else
                        {
                            client.Send(new SystemMessagePacket(
                                $"Invalid command parameters. Check the available commands on the Admin Portal."));
                            break;
                        }
                    }
                    else if (command[1].ToLower() == "clear")
                    {
                        client.Tamer.Warehouse.Clear();

                        client.Send(new SystemMessagePacket($"Storage slots cleaned."));
                        client.Send(new LoadInventoryPacket(client.Tamer.Warehouse, InventoryTypeEnum.Warehouse));

                        await _sender.Send(new UpdateItemsCommand(client.Tamer.Warehouse));
                    }
                }
                    break;

                case "godmode":
                {
                    var regex = @"(godmode\son$){1}|(godmode\soff$){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    if (command[1].ToLower() == "on")
                    {
                        var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

                        switch (mapConfig.Type)
                        {
                            case MapTypeEnum.Pvp:
                                client.Tamer.SetGodMode(false);
                                break;
                            default:
                            {
                                if (client.Tamer.GodMode)
                                {
                                    client.Send(new SystemMessagePacket($"You are already in god mode."));
                                }
                                else
                                {
                                    client.Tamer.SetGodMode(true);
                                    client.Send(new SystemMessagePacket($"God mode enabled."));
                                }
                            }
                                break;
                        }
                    }
                    else
                    {
                        if (!client.Tamer.GodMode)
                        {
                            client.Send(new SystemMessagePacket($"You are already with god mode disabled."));
                        }
                        else
                        {
                            client.Tamer.SetGodMode(false);
                            client.Send(new SystemMessagePacket($"God mode disabled."));
                        }
                    }
                }
                    break;

                case "unlockevos":
                {
                    var regex = @"^unlockevos";
                    var match = Regex.IsMatch(message, regex, RegexOptions.IgnoreCase);

                    if (!match)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !unlockevos"));
                        break;
                    }

                    // Unlock Digimon Evolutions

                    foreach (var evolution in client.Partner.Evolutions)
                    {
                        evolution.Unlock();
                        await _sender.Send(new UpdateEvolutionCommand(evolution));
                    }

                    // Unlock Digimon Evolutions on Encyclopedia

                    var evoInfo = _assets.EvolutionInfo.FirstOrDefault(x => x.Type == client.Partner.BaseType)?.Lines
                        .FirstOrDefault(x => x.Type == client.Partner.CurrentType);

                    if (evoInfo == null)
                    {
                        _logger.Error($"evoInfo not found !! [ Unlockevos Command ]");
                    }
                    else
                    {
                        var encyclopedia =
                            client.Tamer.Encyclopedia.FirstOrDefault(x => x.DigimonEvolutionId == evoInfo.EvolutionId);

                        if (encyclopedia == null)
                        {
                            _logger.Error($"encyclopedia not found !! [ Unlockevos Command ]");
                        }
                        else
                        {
                            foreach (var evolution in client.Partner.Evolutions)
                            {
                                var encyclopediaEvolution =
                                    encyclopedia.Evolutions.First(x => x.DigimonBaseType == evolution.Type);

                                encyclopediaEvolution.Unlock();

                                await _sender.Send(
                                    new UpdateCharacterEncyclopediaEvolutionsCommand(encyclopediaEvolution));
                            }

                            int LockedEncyclopediaCount = encyclopedia.Evolutions.Count(x => x.IsUnlocked == false);

                            if (LockedEncyclopediaCount <= 0)
                            {
                                try
                                {
                                    encyclopedia.SetRewardAllowed();
                                    await _sender.Send(new UpdateCharacterEncyclopediaCommand(encyclopedia));
                                }
                                catch (Exception ex)
                                {
                                    //_logger.Error($"LockedEncyclopediaCount Error:\n{ex.Message}");
                                }
                            }
                        }
                    }

                    // -- RELOADING MAP -----------------------------------------------------------------------------

                    client.Tamer.UpdateState(CharacterStateEnum.Loading);
                    await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

                    _mapServer.RemoveClient(client);

                    client.SetGameQuit(false);
                    client.Tamer.UpdateSlots();

                    client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                        client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y));
                }
                    break;

                case "openseals":
                {
                    var sealInfoList = _assets.SealInfo;
                    foreach (var seal in sealInfoList)
                    {
                        client.Tamer.SealList.AddOrUpdateSeal(seal.SealId, 3000, seal.SequentialId);
                    }

                    client.Partner?.SetSealStatus(sealInfoList);

                    client.Send(new UpdateStatusPacket(client.Tamer));

                    await _sender.Send(new UpdateCharacterSealsCommand(client.Tamer.SealList));

                    client.Tamer.UpdateState(CharacterStateEnum.Loading);
                    await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

                    _mapServer.RemoveClient(client);

                    client.SetGameQuit(false);

                    client.Send(new MapSwapPacket(
                        _configuration[GamerServerPublic],
                        _configuration[GameServerPort],
                        client.Tamer.Location.MapId,
                        client.Tamer.Location.X,
                        client.Tamer.Location.Y));
                }
                    break;

                case "su":
                {
                    var regex = @"^su\s\d+(\s\d+)?$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !summon MobId"));
                        break;
                    }

                    var mobType = int.Parse(command[1].ToLower());

                    var MobInfo = _assets.SummonMobInfo.FirstOrDefault(x => x.Type == mobType);

                    if (MobInfo != null)
                    {
                        var mob = (SummonMobModel)MobInfo.Clone();

                        _logger.Information($"mob {mob.Id} : {mob.Type} : {mob.Name} being summoned !!");

                        try
                        {
                            int radius = 500;
                            var random = new Random();

                            int xOffset = random.Next(-radius, radius + 1);
                            int yOffset = random.Next(-radius, radius + 1);

                            int bossX = client.Tamer.Location.X + xOffset;
                            int bossY = client.Tamer.Location.Y + yOffset;

                            //var map = _mapServer.Maps.FirstOrDefault(x => x.Clients.Exists(gameClient => gameClient.TamerId == client.TamerId));

                            //var mobId = mob.Id;

                            //mob.SetId(mobId);

                            if (mob?.Location?.X != 0 && mob?.Location?.Y != 0)
                            {
                                bossX = mob.Location.X;
                                bossY = mob.Location.Y;

                                mob.SetLocation(client.Tamer.Location.MapId, bossX, bossY);
                            }
                            else
                            {
                                mob.SetLocation(client.Tamer.Location.MapId, bossX, bossY);
                            }

                            mob.SetDuration();
                            mob.SetTargetSummonHandle(client.Tamer.GeneralHandler);

                            _mapServer.AddSummonMob(client.Tamer.Location.MapId, mob, client.TamerId);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"{ex.Message}");
                        }

                        _logger.Information($"mob {mob.Type} : {mob.Name} spawned !!");
                    }
                    else
                    {
                        client.Send(new SystemMessagePacket($"Invalid Mob Type !!"));
                    }
                }
                    break;

                case "summon":
                {
                    var regex = @"(summon\s\d\s\d){1}|(summon\s\d){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !summon MobId"));
                        break;
                    }

                    var mobId = int.Parse(command[1].ToLower());

                    var SummonInfo = _assets.SummonInfo.FirstOrDefault(x => x.ItemId == 27100);

                    if (SummonInfo != null)
                    {
                        await SummonMonster(client, SummonInfo);
                    }
                    else
                    {
                        client.Send(new SystemMessagePacket($"Invalid MobId !!"));
                    }
                }
                    break;

                case "heal":
                {
                    var regex = @"^heal\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !heal"));
                        break;
                    }

                    client.Tamer.FullHeal();
                    client.Tamer.Partner.FullHeal();

                    client.Send(new UpdateStatusPacket(client.Tamer));
                    await _sender.Send(new UpdateCharacterBasicInfoCommand(client.Tamer));
                }
                    break;

                case "stats":
                {
                    var regex = @"^stats\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !stats"));
                        break;
                    }

                    client.Send(new SystemMessagePacket($"Critical Damage: {client.Tamer.Partner.CD / 100}%\n" +
                                                        $"Attribute Damage: {client.Tamer.Partner.ATT / 100}%\n" +
                                                        $"Digimon SKD: {client.Tamer.Partner.SKD}\n" +
                                                        $"Digimon SCD: {client.Tamer.Partner.SCD / 100}%\n" +
                                                        $"Tamer BonusEXP: {client.Tamer.BonusEXP}%\n" +
                                                        $"Tamer Move Speed: {client.Tamer.MS}", ""));
                }
                    break;

                case "encyclopedia":
                {
                    var regex = @"(encyclopedia\s\d\s\d){1}|(encyclopedia\s\d){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !encyclopedia"));
                        break;
                    }

                    int type = int.Parse(command[1].ToLower());
                    // DigimonBaseInfoAssetModel digimon = _mapper.Map<DigimonBaseInfoAssetModel>(await _sender.Send(new DigimonBaseInfoQuery(type)));

                    var digimonEvolutionInfo = _assets.EvolutionInfo.FirstOrDefault(x => x.Type == type);

                    _logger.Information($"type: {type}, info: {digimonEvolutionInfo?.ToString()}");

                    if (digimonEvolutionInfo == null)
                    {
                        client.Send(new SystemMessagePacket($"evolution info not found"));
                        return;
                    }

                    List<EvolutionLineAssetModel> evolutionLines =
                        digimonEvolutionInfo.Lines.OrderBy(x => x.Id).ToList();

                    var encyclopedia = CharacterEncyclopediaModel.Create(client.TamerId, digimonEvolutionInfo.Id, 120,
                        14000, 15, 15, 15, 15, 15, false, false);

                    evolutionLines?.ForEach(x =>
                    {
                        encyclopedia.Evolutions.Add(
                            CharacterEncyclopediaEvolutionsModel.Create(x.Type, x.SlotLevel, false));
                    });

                    client.Tamer.Encyclopedia.Add(encyclopedia);

                    var encyclopediaAdded = await _sender.Send(new CreateCharacterEncyclopediaCommand(encyclopedia));

                    client.Send(new SystemMessagePacket($"Encyclopedia added! {encyclopediaAdded.Id}, evolutions"));
                }
                    break;

                // -- TOOLS --------------------------------------

                #region Tools

                case "tools":
                {
                    var regex = @"^tools\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !tools"));
                        break;
                    }

                    client.Send(new SystemMessagePacket($"Tools Commands:", "").Serialize());
                    client.Send(
                        new SystemMessagePacket($"1. !fullacc\n2. !evopack\n3. !spacepack\n4. !clon (type) (value)", "")
                            .Serialize());
                }
                    break;

                case "fullacc":
                {
                    await AddItemToInventory(client, 50, 1); // 
                    await AddItemToInventory(client, 89143, 1); // 
                    await AddItemToInventory(client, 40011, 1); // 
                    await AddItemToInventory(client, 41038, 1); // Jogress Chip
                    await AddItemToInventory(client, 131063, 1); // XAI Ver VI
                    await AddItemToInventory(client, 41113, 1); // DigiAuraBox
                    await AddItemToInventory(client, 41002, 50); // Accelerator
                    await AddItemToInventory(client, 71594, 20); // X-Antibody

                    #region BITS (100T)

                    client.Tamer.Inventory.AddBits(100000000);

                    client.Send(
                        new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize());

                    await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory.Id,
                        client.Tamer.Inventory.Bits));

                    #endregion
                }
                    break;

                case "evopack":
                {
                    var regex = @"^evopack\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !evopack"));
                        break;
                    }

                    await AddItemToInventory(client, 41002, 20); // Accelerator
                    await AddItemToInventory(client, 41000, 20); // Spirit Accelerator
                    await AddItemToInventory(client, 5001, 20); // Evoluter
                    await AddItemToInventory(client, 71594, 20); // X-Antibody

                    client.Send(
                        new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize());
                    client.Send(new SystemMessagePacket($"Items for evo on inventory!!"));
                }
                    break;

                case "spacepack":
                {
                    var regex = @"^spacepack\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !spacepack"));
                        break;
                    }

                    await AddItemToInventory(client, 5507, 10); // Inventory Expansion
                    await AddItemToInventory(client, 5508, 10); // Warehouse Expansion
                    await AddItemToInventory(client, 5004, 10); // Archive Expansion
                    await AddItemToInventory(client, 5812, 2); // Digimon Slot

                    client.Send(
                        new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize());
                    client.Send(new SystemMessagePacket($"Items for space on inventory!!"));
                }
                    break;

                case "clon":
                {
                    var cloneAT = (DigicloneTypeEnum)1;
                    var cloneBL = (DigicloneTypeEnum)2;
                    var cloneCT = (DigicloneTypeEnum)3;
                    var cloneEV = (DigicloneTypeEnum)5;
                    var cloneHP = (DigicloneTypeEnum)7;

                    if (command.Length < 2)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !clon type value"));
                        break;
                    }

                    int maxCloneLevel = 15;

                    if (command.Length > 2 && int.TryParse(command[2], out int requestedLevel))
                    {
                        maxCloneLevel = Math.Min(requestedLevel, 15);
                    }

                    async Task IncreaseCloneLevel(DigicloneTypeEnum cloneType, string cloneName)
                    {
                        var currentCloneLevel = client.Partner.Digiclone.GetCurrentLevel(cloneType);

                        while (currentCloneLevel < maxCloneLevel)
                        {
                            var cloneAsset = _assets.CloneValues.FirstOrDefault(x =>
                                x.Type == cloneType && currentCloneLevel + 1 >= x.MinLevel &&
                                currentCloneLevel + 1 <= x.MaxLevel);

                            if (cloneAsset != null)
                            {
                                var cloneResult = DigicloneResultEnum.Success;
                                short value = (short)cloneAsset.MaxValue;

                                client.Partner.Digiclone.IncreaseCloneLevel(cloneType, value);

                                client.Send(new DigicloneResultPacket(cloneResult, client.Partner.Digiclone));
                                client.Send(new UpdateStatusPacket(client.Tamer));

                                await _sender.Send(new UpdateDigicloneCommand(client.Partner.Digiclone));

                                currentCloneLevel++;
                                _logger.Verbose($"New {cloneName} Clon Level: {currentCloneLevel}");
                            }
                            else
                            {
                                break;
                            }
                        }

                        client.Send(new SystemMessagePacket($"New {cloneName} Clon Level: {currentCloneLevel}"));
                    }

                    switch (command[1].ToLower())
                    {
                        case "at":
                        {
                            await IncreaseCloneLevel(cloneAT, "AT");
                        }
                            break;

                        case "bl":
                        {
                            await IncreaseCloneLevel(cloneBL, "BL");
                        }
                            break;

                        case "ct":
                        {
                            await IncreaseCloneLevel(cloneCT, "CT");
                        }
                            break;

                        case "hp":
                        {
                            await IncreaseCloneLevel(cloneHP, "HP");
                        }
                            break;

                        case "ev":
                        {
                            await IncreaseCloneLevel(cloneEV, "EV");
                        }
                            break;

                        default:
                        {
                            client.Send(new SystemMessagePacket("Unknown command.\nType !clon type value"));
                        }
                            break;
                    }
                }
                    break;

                case "maptamers":
                {
                    var regex = @"^maptamers\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !maptamers"));
                        break;
                    }

                    var mapTamers =
                        _mapServer.Maps.FirstOrDefault(x =>
                            x.Clients.Exists(gameClient => gameClient.TamerId == client.Tamer.Id));

                    if (mapTamers != null)
                    {
                        client.Send(new SystemMessagePacket($"Total Tamers in Map: {mapTamers.ConnectedTamers.Count}",
                            ""));
                    }
                    else
                    {
                        mapTamers = _dungeonServer.Maps.FirstOrDefault(x =>
                            x.Clients.Exists(gameClient => gameClient.TamerId == client.Tamer.Id));

                        client.Send(
                            new SystemMessagePacket($"Total Tamers in Dungeon Map: {mapTamers.ConnectedTamers.Count}",
                                ""));
                    }
                }
                    break;

                #endregion

                // -- INFO ---------------------------------------

                #region INFO

                case "updatestats":
                {
                    var regex = @"^updatestats\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !updatestats"));
                        break;
                    }

                    client.Send(new UpdateStatusPacket(client.Tamer));

                    client.Send(new SystemMessagePacket($"Stats updated !!"));
                }
                    break;

                case "time":
                {
                    var regex = @"^time\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !time"));
                        break;
                    }

                    client.Send(new SystemMessagePacket($"Server Time is: {DateTime.UtcNow}"));
                }
                    break;

                case "deckload":
                {
                    var regex = @"^deckload\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !deckload"));
                        break;
                    }

                    var evolution = client.Partner.Evolutions[0];

                    _logger.Information(
                        $"Evolution ID: {evolution.Id} | Evolution Type: {evolution.Type} | Evolution Unlocked: {evolution.Unlocked}");

                    var evoInfo = _assets.EvolutionInfo.FirstOrDefault(x => x.Type == client.Partner.BaseType)?.Lines
                        .FirstOrDefault(x => x.Type == evolution.Type);

                    _logger.Information($"EvoInfo ID: {evoInfo.Id}");
                    _logger.Information($"EvoInfo EvolutionId: {evoInfo.EvolutionId}");

                    // --- CREATE DB ----------------------------------------------------------------------------------------

                    var digimonEvolutionInfo = _assets.EvolutionInfo.First(x => x.Type == client.Partner.BaseType);

                    var digimonEvolutions = client.Partner.Evolutions;

                    var encyclopediaExists =
                        client.Tamer.Encyclopedia.Exists(x => x.DigimonEvolutionId == digimonEvolutionInfo.Id);

                    if (!encyclopediaExists)
                    {
                        if (digimonEvolutionInfo != null)
                        {
                            var newEncyclopedia = CharacterEncyclopediaModel.Create(client.TamerId,
                                digimonEvolutionInfo.Id, client.Partner.Level, client.Partner.Size, 0, 0, 0, 0, 0,
                                false, false);

                            digimonEvolutions?.ForEach(x =>
                            {
                                var evolutionLine = digimonEvolutionInfo.Lines.FirstOrDefault(y => y.Type == x.Type);

                                byte slotLevel = 0;

                                if (evolutionLine != null)
                                    slotLevel = evolutionLine.SlotLevel;

                                newEncyclopedia.Evolutions.Add(
                                    CharacterEncyclopediaEvolutionsModel.Create(newEncyclopedia.Id, x.Type, slotLevel,
                                        Convert.ToBoolean(x.Unlocked)));
                            });

                            var encyclopediaAdded =
                                await _sender.Send(new CreateCharacterEncyclopediaCommand(newEncyclopedia));

                            client.Tamer.Encyclopedia.Add(encyclopediaAdded);

                            _logger.Information($"Digimon Type {client.Partner.BaseType} encyclopedia created !!");
                        }
                    }
                    else
                    {
                        _logger.Information($"Encyclopedia already exist !!");
                    }

                    // --- UNLOCK -------------------------------------------------------------------------------------------

                    var encyclopedia =
                        client.Tamer.Encyclopedia.First(x => x.DigimonEvolutionId == evoInfo.EvolutionId);

                    _logger.Information($"Encyclopedia is: {encyclopedia.Id}, evolution id: {evoInfo.EvolutionId}");

                    if (encyclopedia != null)
                    {
                        var encyclopediaEvolution =
                            encyclopedia.Evolutions.First(x => x.DigimonBaseType == evolution.Type);

                        if (!encyclopediaEvolution.IsUnlocked)
                        {
                            encyclopediaEvolution.Unlock();

                            await _sender.Send(new UpdateCharacterEncyclopediaEvolutionsCommand(encyclopediaEvolution));

                            int LockedEncyclopediaCount = encyclopedia.Evolutions.Count(x => x.IsUnlocked == false);

                            if (LockedEncyclopediaCount <= 0)
                            {
                                encyclopedia.SetRewardAllowed();
                                await _sender.Send(new UpdateCharacterEncyclopediaCommand(encyclopedia));
                            }
                        }
                        else
                        {
                            _logger.Information($"Evolution already unlocked on encyclopedia !!");
                        }
                    }

                    // ------------------------------------------------------------------------------------------------------

                    client.Send(new SystemMessagePacket($"Encyclopedia verifyed and updated !!"));
                }
                    break;

                #endregion

                // -- MAINTENANCE --------------------------------

                #region Maintenance

                case "live":
                {
                    if (client.AccessLevel == AccountAccessLevelEnum.Administrator)
                    {
                        var packet = new PacketWriter();
                        packet.Type(1006);
                        packet.WriteByte(10);
                        packet.WriteByte(1);
                        packet.WriteString("Server is now on live!");
                        packet.WriteByte(0);

                        _mapServer.BroadcastGlobal(packet.Serialize());

                        var server = await _sender.Send(new GetServerByIdQuery(client.ServerId));
                        if (server.Register != null)
                            await _sender.Send(new UpdateServerCommand(server.Register.Id, server.Register.Name,
                                server.Register.Experience, false));
                    }
                    else
                    {
                        client.Send(new SystemMessagePacket("Insufficient permission level for this action."));
                    }
                }
                    break;

                case "maintenance":
                {
                    if (client.AccessLevel == AccountAccessLevelEnum.Administrator)
                    {
                        var packet = new PacketWriter();
                        packet.Type(1006);
                        packet.WriteByte(10);
                        packet.WriteByte(1);
                        packet.WriteString("Server shutdown for maintenance in 2 minutes");
                        packet.WriteByte(0);

                        _mapServer.BroadcastGlobal(packet.Serialize());
                        var ServerId = client.ServerId;
                        var server = await _sender.Send(new GetServerByIdQuery(client.ServerId));
                        if (server.Register != null)
                            await _sender.Send(new UpdateServerCommand(server.Register.Id, server.Register.Name,
                                server.Register.Experience, true));


                        Task task = Task.Run(async () =>
                        {
                            Thread.Sleep(60000);
                            var packetWriter = new PacketWriter();
                            packetWriter.Type(1006);
                            packetWriter.WriteByte(10);
                            packetWriter.WriteByte(1);
                            packetWriter.WriteString("Server shutdown for maintenance in 60s");
                            packetWriter.WriteByte(0);
                            _mapServer.BroadcastGlobal(packetWriter.Serialize());
                            _dungeonServer.BroadcastGlobal(packetWriter.Serialize());

                            Thread.Sleep(30000);
                            packetWriter = new PacketWriter();
                            packetWriter.Type(1006);
                            packetWriter.WriteByte(10);
                            packetWriter.WriteByte(1);
                            packetWriter.WriteString("Server shutdown for maintenance in 30s");
                            packetWriter.WriteByte(0);
                            _mapServer.BroadcastGlobal(packetWriter.Serialize());
                            _dungeonServer.BroadcastGlobal(packetWriter.Serialize());

                            Thread.Sleep(20000);
                            for (int i = 10; i >= 0; i--)
                            {
                                Thread.Sleep(1000);
                                packetWriter = new PacketWriter();
                                packetWriter.Type(1006);
                                packetWriter.WriteByte(10);
                                packetWriter.WriteByte(1);
                                packetWriter.WriteString($"Server shutdown for maintenance in {i}s");
                                packetWriter.WriteByte(0);

                                _mapServer.BroadcastGlobal(packetWriter.Serialize());
                                _dungeonServer.BroadcastGlobal(packetWriter.Serialize());
                            }

                            var currentServer = await _sender.Send(new GetServerByIdQuery(ServerId));
                            if (currentServer.Register.Maintenance)
                            {
                                _mapServer.BroadcastGlobal(new DisconnectUserPacket("Server maintenance").Serialize());
                                _dungeonServer.BroadcastGlobal(
                                    new DisconnectUserPacket("Server maintenance").Serialize());
                            }
                        });
                    }
                    else
                    {
                        client.Send(new SystemMessagePacket("Insufficient permission level for this action."));
                    }
                }
                    break;

                #endregion

                // -- MESSAGE ------------------------------------

                #region Messages

                case "notice":
                {
                    var notice = string.Join(" ", message.Split(' ').Skip(1));
                    var packet = new PacketWriter();
                    packet.Type(1006);
                    packet.WriteByte(10);
                    packet.WriteByte(1);
                    packet.WriteString($"{notice}");
                    packet.WriteByte(0);

                    _mapServer.BroadcastGlobal(packet.Serialize());
                }
                    break;

                case "ann":
                {
                    var notice = string.Join(" ", message.Split(' ').Skip(1));
                    _dungeonServer.BroadcastGlobal(
                        new ChatMessagePacket(notice, ChatTypeEnum.Megaphone, "[System]", 52, 120).Serialize());
                    _mapServer.BroadcastGlobal(
                        new ChatMessagePacket(notice, ChatTypeEnum.Megaphone, "[System]", 52, 120).Serialize());
                }
                    break;

                #endregion

                // -- MEMBERSHIP ---------------------------------

                #region Membership

                case "membership":
                {
                    var regex = @"membership\s(add|remove)(\s\d{1,9})?$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    switch (command[1].ToLower())
                    {
                        case "add":
                        {
                            var valueInHours = int.Parse(command[2]);

                            var value = valueInHours * 24 * 3600;

                            client.IncreaseMembershipDuration(value);

                            var buff = _assets.BuffInfo
                                .Where(x => x.BuffId == 50121 || x.BuffId == 50122 || x.BuffId == 50123).ToList();

                            int duration = client.MembershipUtcSecondsBuff;

                            client.Send(new MembershipPacket(client.MembershipExpirationDate!.Value, duration));

                            await _sender.Send(new UpdateAccountMembershipCommand(client.AccountId,
                                client.MembershipExpirationDate));

                            buff.ForEach(buffAsset =>
                            {
                                if (!client.Tamer.BuffList.Buffs.Any(x => x.BuffId == buffAsset.BuffId))
                                {
                                    var newCharacterBuff = CharacterBuffModel.Create(buffAsset.BuffId,
                                        buffAsset.SkillId, 2592000, duration);

                                    newCharacterBuff.SetBuffInfo(buffAsset);

                                    client.Tamer.BuffList.Buffs.Add(newCharacterBuff);

                                    _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                        new AddBuffPacket(client.Tamer.GeneralHandler, buffAsset, 0, duration)
                                            .Serialize());
                                }
                                else
                                {
                                    var buffData = client.Tamer.BuffList.Buffs.First(x => x.BuffId == buffAsset.BuffId);

                                    if (buffData != null)
                                    {
                                        buffData.SetDuration(duration, true);

                                        _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                            new UpdateBuffPacket(client.Tamer.GeneralHandler, buffAsset, 0, duration)
                                                .Serialize());
                                    }
                                }
                            });

                            await _sender.Send(new UpdateCharacterBuffListCommand(client.Tamer.BuffList));

                            //client.Send(new MembershipPacket(client.MembershipExpirationDate!.Value, duration));
                            client.Send(new UpdateStatusPacket(client.Tamer));

                            // -- RELOAD -------------------------

                            client.Tamer.UpdateState(CharacterStateEnum.Loading);
                            await _sender.Send(new UpdateCharacterStateCommand(client.TamerId,
                                CharacterStateEnum.Loading));

                            _mapServer.RemoveClient(client);

                            client.SetGameQuit(false);
                            client.Tamer.UpdateSlots();

                            client.Send(new MapSwapPacket(_configuration[GamerServerPublic],
                                _configuration[GameServerPort],
                                client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y));
                        }
                            break;

                        case "remove":
                        {
                            client.RemoveMembership();

                            int duration = client.MembershipUtcSecondsBuff;

                            client.Send(new MembershipPacket());

                            await _sender.Send(new UpdateAccountMembershipCommand(client.AccountId,
                                client.MembershipExpirationDate));

                            var secondsUTC = (client.MembershipExpirationDate.Value - DateTime.UtcNow).TotalSeconds;

                            if (secondsUTC <= 0)
                            {
                                //_logger.Information($"Verifying if tamer have buffs without membership");

                                var buff1 = _assets.BuffInfo.FirstOrDefault(x => x.BuffId == 50121);
                                var buff2 = _assets.BuffInfo.FirstOrDefault(x => x.BuffId == 50122);
                                var buff3 = _assets.BuffInfo.FirstOrDefault(x => x.BuffId == 50123);

                                var characterBuff1 =
                                    client.Tamer.BuffList.ActiveBuffs.FirstOrDefault(x => x.BuffId == buff1.BuffId);
                                var characterBuff2 =
                                    client.Tamer.BuffList.ActiveBuffs.FirstOrDefault(x => x.BuffId == buff2.BuffId);
                                var characterBuff3 =
                                    client.Tamer.BuffList.ActiveBuffs.FirstOrDefault(x => x.BuffId == buff3.BuffId);

                                if (characterBuff1 != null)
                                {
                                    client.Tamer.BuffList.Buffs.Remove(characterBuff1);

                                    client.Send(new UpdateStatusPacket(client.Tamer));
                                    client.Send(new RemoveBuffPacket(client.Tamer.GeneralHandler, buff1.BuffId)
                                        .Serialize());
                                }

                                if (characterBuff2 != null)
                                {
                                    client.Tamer.BuffList.Buffs.Remove(characterBuff2);

                                    client.Send(new UpdateStatusPacket(client.Tamer));
                                    client.Send(new RemoveBuffPacket(client.Tamer.GeneralHandler, buff2.BuffId)
                                        .Serialize());
                                }

                                if (characterBuff3 != null)
                                {
                                    client.Tamer.BuffList.Buffs.Remove(characterBuff3);

                                    client.Send(new UpdateStatusPacket(client.Tamer));
                                    client.Send(new RemoveBuffPacket(client.Tamer.GeneralHandler, buff3.BuffId)
                                        .Serialize());
                                }

                                await _sender.Send(new UpdateCharacterBuffListCommand(client.Tamer.BuffList));
                            }

                            client.Send(new UpdateStatusPacket(client.Tamer));

                            // -- RELOAD -------------------------

                            client.Tamer.UpdateState(CharacterStateEnum.Loading);
                            await _sender.Send(new UpdateCharacterStateCommand(client.TamerId,
                                CharacterStateEnum.Loading));

                            _mapServer.RemoveClient(client);

                            client.SetGameQuit(false);
                            client.Tamer.UpdateSlots();

                            client.Send(new MapSwapPacket(_configuration[GamerServerPublic],
                                _configuration[GameServerPort],
                                client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y));
                        }
                            break;

                        default:
                            client.Send(
                                new SystemMessagePacket(
                                    $"Unknown command. Check the available commands on the Admin Portal."));
                            break;
                    }
                }
                    break;

                #endregion

                // -- LOCATION -----------------------------------

                #region Location

                case "where":
                {
                    var regex = @"(where$){1}|(location$){1}|(position$){1}|(pos$){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    var loc = client.Tamer.Location;
                    var ch = client.Tamer.Channel;

                    var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(loc.MapId));

                    client.Send(
                        new SystemMessagePacket(
                            $"Map {loc.MapId} Ch {ch} (X: {loc.X}, Y: {loc.Y})\nServer: {mapConfig.Type}"));
                }
                    break;

                case "tp":
                {
                    var regex = @"(tp\s\d\s\d){1}|(tp\s\d){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    var playerMap = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

                    try
                    {
                        var mapId = Convert.ToInt32(command[1].ToLower());
                        var waypoint = command.Length == 3 ? Convert.ToInt32(command[2]) : 0;

                        var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(mapId));
                        var waypoints = await _sender.Send(new MapRegionListAssetsByMapIdQuery(mapId));

                        if (mapConfig == null)
                        {
                            client.Send(new SystemMessagePacket($"Config Map not found for MapID: {mapId}"));
                            break;
                        }
                        else if (waypoints == null || !waypoints.Regions.Any())
                        {
                            client.Send(
                                new SystemMessagePacket($"Map Region information not found for MapID: {mapId}"));
                            break;
                        }

                        switch (playerMap.Type)
                        {
                            case MapTypeEnum.Dungeon:
                                _dungeonServer.RemoveClient(client);
                                break;
                            case MapTypeEnum.Event:
                                _eventServer.RemoveClient(client);
                                break;
                            case MapTypeEnum.Pvp:
                                _pvpServer.RemoveClient(client);
                                break;
                            case MapTypeEnum.Default:
                                _mapServer.RemoveClient(client);
                                break;
                        }

                        var destination = waypoints.Regions.First();

                        client.Tamer.NewLocation(mapId, destination.X, destination.Y);
                        await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));

                        client.Tamer.Partner.NewLocation(mapId, destination.X, destination.Y);
                        await _sender.Send(new UpdateDigimonLocationCommand(client.Tamer.Partner.Location));

                        client.Tamer.UpdateState(CharacterStateEnum.Loading);
                        await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

                        client.SetGameQuit(false);

                        client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                            client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y).Serialize());
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"TP Error:\n {ex.Message}");
                    }

                    var party = _partyManager.FindParty(client.TamerId);

                    if (party != null)
                    {
                        party.UpdateMember(party[client.TamerId], client.Tamer);

                        /*foreach (var target in party.Members.Values)
                        {
                            var targetClient = _mapServer.FindClientByTamerId(target.Id);

                            if (targetClient == null) targetClient = _dungeonServer.FindClientByTamerId(target.Id);

                            if (targetClient == null) continue;

                            if (target.Id != client.Tamer.Id)
                                targetClient.Send(new PartyMemberWarpGatePacket(party[client.TamerId], targetClient.Tamer).Serialize());
                        }*/

                        _mapServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[client.TamerId], client.Tamer).Serialize());

                        _dungeonServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[client.TamerId], client.Tamer).Serialize());

                        _eventServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[client.TamerId], client.Tamer).Serialize());

                        _pvpServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[client.TamerId], client.Tamer).Serialize());
                    }
                }
                    break;

                case "tpto":
                {
                    var regex = @"^tpto\s[\w\s]+$";
                    var match = Regex.Match(message, regex, RegexOptions.None);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !tpto (TamerName)"));
                        break;
                    }

                    string[] comando = message.Split(' ');
                    var TamerName = comando[1];

                    GameClient? targetClient;

                    var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

                    if (client.Tamer.Name == TamerName)
                    {
                        client.Send(new SystemMessagePacket($"You can't teleport to yourself!"));
                        break;
                    }

                    var map = _mapServer.Maps.FirstOrDefault(x => x.Clients.Exists(x => x.Tamer.Name == TamerName));
                    var mapD = _dungeonServer.Maps.FirstOrDefault(x =>
                        x.Clients.Exists(x => x.Tamer.Name == TamerName));
                    var mapE = _eventServer.Maps.FirstOrDefault(x => x.Clients.Exists(x => x.Tamer.Name == TamerName));
                    var mapP = _pvpServer.Maps.FirstOrDefault(x => x.Clients.Exists(x => x.Tamer.Name == TamerName));

                    if (map != null)
                    {
                        targetClient = _mapServer.FindClientByTamerName(TamerName);
                    }
                    else if (mapD != null)
                    {
                        targetClient = _dungeonServer.FindClientByTamerName(TamerName);
                    }
                    else if (mapE != null)
                    {
                        targetClient = _eventServer.FindClientByTamerName(TamerName);
                    }
                    else if (mapP != null)
                    {
                        targetClient = _pvpServer.FindClientByTamerName(TamerName);
                    }
                    else
                    {
                        client.Send(new SystemMessagePacket($"Player {TamerName} not found !"));
                        break;
                    }

                    switch (mapConfig.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            _dungeonServer.RemoveClient(client);
                            break;
                        case MapTypeEnum.Event:
                            _eventServer.RemoveClient(client);
                            break;
                        case MapTypeEnum.Pvp:
                            _pvpServer.RemoveClient(client);
                            break;
                        case MapTypeEnum.Default:
                            _mapServer.RemoveClient(client);
                            break;
                    }

                    var destination = targetClient.Tamer.Location;

                    client.Tamer.SetTamerTP(targetClient.TamerId);
                    await _sender.Send(new ChangeTamerIdTPCommand(client.Tamer.Id, (int)targetClient.TamerId));

                    client.Tamer.NewLocation(destination.MapId, destination.X, destination.Y);
                    await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));

                    client.Tamer.Partner.NewLocation(destination.MapId, destination.X, destination.Y);
                    await _sender.Send(new UpdateDigimonLocationCommand(client.Tamer.Partner.Location));

                    client.Tamer.SetCurrentChannel(targetClient.Tamer.Channel);

                    client.Tamer.UpdateState(CharacterStateEnum.Loading);
                    await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

                    client.SetGameQuit(false);

                    client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                        client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y).Serialize());

                    var party = _partyManager.FindParty(client.TamerId);

                    if (party != null)
                    {
                        party.UpdateMember(party[client.TamerId], client.Tamer);

                        /*party.Members.Values.Where(x => x.Id != client.TamerId).ToList().ForEach(member =>
                        {
                            _dungeonServer.BroadcastForUniqueTamer(member.Id, new PartyMemberWarpGatePacket(party[client.TamerId], member).Serialize());
                        });*/

                        _mapServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[client.TamerId], client.Tamer).Serialize());

                        _dungeonServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[client.TamerId], client.Tamer).Serialize());

                        _eventServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[client.TamerId], client.Tamer).Serialize());

                        _pvpServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[client.TamerId], client.Tamer).Serialize());
                    }
                }
                    break;

                case "tptamer":
                {
                    var regex = @"^tptamer\s[\w\s]+$";
                    var match = Regex.Match(message, regex, RegexOptions.None);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !tptamer (TamerName)"));
                        break;
                    }

                    if (command.Length < 2)
                    {
                        client.Send(new SystemMessagePacket("Invalid command format.\nType !tptamer (TamerName)"));
                        break;
                    }

                    var tamerName = command[1];

                    GameClient? TargetSummon;

                    var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(client.Tamer.Location.MapId));

                    switch (mapConfig!.Type)
                    {
                        case MapTypeEnum.Dungeon:
                            TargetSummon = _dungeonServer.FindClientByTamerName(tamerName);
                            break;
                        case MapTypeEnum.Event:
                            TargetSummon = _eventServer.FindClientByTamerName(tamerName);
                            break;
                        case MapTypeEnum.Pvp:
                            TargetSummon = _pvpServer.FindClientByTamerName(tamerName);
                            break;
                        default:
                            TargetSummon = _mapServer.FindClientByTamerName(tamerName);
                            break;
                    }

                    if (TargetSummon == null)
                    {
                        client.Send(new SystemMessagePacket($"Tamer {tamerName} is not online!"));
                        return;
                    }

                    _logger.Information($"GM: {client.Tamer.Name} teleported Tamer: {TargetSummon.Tamer.Name}");

                    var mapId = client.Tamer.Location.MapId;
                    var destination = client.Tamer.Location;

                    if (TargetSummon.DungeonMap)
                        _dungeonServer.RemoveClient(TargetSummon);
                    else if (TargetSummon.EventMap)
                        _eventServer.RemoveClient(TargetSummon);
                    else if (TargetSummon.PvpMap)
                        _pvpServer.RemoveClient(TargetSummon);
                    else
                        _mapServer.RemoveClient(TargetSummon);

                    TargetSummon.Tamer.NewLocation(mapId, destination.X, destination.Y);
                    await _sender.Send(new UpdateCharacterLocationCommand(TargetSummon.Tamer.Location));

                    TargetSummon.Tamer.Partner.NewLocation(mapId, destination.X, destination.Y);
                    await _sender.Send(new UpdateDigimonLocationCommand(TargetSummon.Tamer.Partner.Location));

                    TargetSummon.Tamer.UpdateState(CharacterStateEnum.Loading);
                    await _sender.Send(
                        new UpdateCharacterStateCommand(TargetSummon.TamerId, CharacterStateEnum.Loading));

                    TargetSummon.SetGameQuit(false);

                    TargetSummon.Send(new MapSwapPacket(_configuration[GamerServerPublic],
                        _configuration[GameServerPort],
                        client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y));

                    var party = _partyManager.FindParty(TargetSummon.TamerId);

                    if (party != null)
                    {
                        party.UpdateMember(party[TargetSummon.TamerId], TargetSummon.Tamer);

                        _mapServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[TargetSummon.TamerId], client.Tamer).Serialize());

                        _dungeonServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[TargetSummon.TamerId], client.Tamer).Serialize());

                        _eventServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[TargetSummon.TamerId], client.Tamer).Serialize());

                        _pvpServer.BroadcastForTargetTamers(party.GetMembersIdList(),
                            new PartyMemberWarpGatePacket(party[TargetSummon.TamerId], client.Tamer).Serialize());
                    }

                    client.Send(new SystemMessagePacket($"You teleported Tamer: {TargetSummon.Tamer.Name}"));
                    TargetSummon.Send(new SystemMessagePacket($"You have been teleported by GM: {client.Tamer.Name}"));
                }
                    break;

                case "exit":
                {
                    var regex = @"^exit\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(
                            new SystemMessagePacket(
                                $"Unknown command. Check the available commands on the Admin Portal."));
                        break;
                    }

                    if (client.Tamer.Location.MapId == 89)
                    {
                        var mapId = 3;

                        var mapConfig = await _sender.Send(new GameMapConfigByMapIdQuery(mapId));
                        var waypoints = await _sender.Send(new MapRegionListAssetsByMapIdQuery(mapId));

                        if (client.DungeonMap)
                            _dungeonServer.RemoveClient(client);
                        else
                            _mapServer.RemoveClient(client);

                        var destination = waypoints.Regions.First();

                        client.Tamer.NewLocation(mapId, destination.X, destination.Y);
                        await _sender.Send(new UpdateCharacterLocationCommand(client.Tamer.Location));

                        client.Tamer.Partner.NewLocation(mapId, destination.X, destination.Y);
                        await _sender.Send(new UpdateDigimonLocationCommand(client.Tamer.Partner.Location));

                        client.Tamer.UpdateState(CharacterStateEnum.Loading);
                        await _sender.Send(new UpdateCharacterStateCommand(client.TamerId, CharacterStateEnum.Loading));

                        client.SetGameQuit(false);

                        client.Send(new MapSwapPacket(_configuration[GamerServerPublic], _configuration[GameServerPort],
                            client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y).Serialize());
                    }
                    else
                    {
                        client.Send(new SystemMessagePacket($"Este comando so pode ser usado no Mapa de Evento !!"));
                        break;
                    }
                }
                    break;

                #endregion

                // -- BUFF ---------------------------------------

                #region Buff

                case "buff":
                {
                    var regex = @"buff\s(add|remove)\s\d+";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !buff (add/remove)"));
                        break;
                    }

                    if (command.Length < 3)
                    {
                        client.Send(
                            new SystemMessagePacket("Invalid command format.\nType !buff (add/remove) (buffID)"));
                        break;
                    }

                    if (!int.TryParse(command[2], out var buffId))
                    {
                        client.Send(new SystemMessagePacket("Invalid item ID format. Please provide a valid number."));
                        break;
                    }

                    var buff = _assets.BuffInfo.FirstOrDefault(x => x.BuffId == buffId);

                    if (buff != null)
                    {
                        var duration = 0;

                        if (command[1].ToLower() == "add")
                        {
                            // Verify if is Tamer Skill
                            if (buff.SkillCode > 0)
                            {
                                if (client.Tamer.BuffList.Buffs.Any(x => x.BuffId == buff.BuffId))
                                {
                                    client.Send(new SystemMessagePacket($"You already have this buff !!"));
                                    break;
                                }

                                var newCharacterBuff =
                                    CharacterBuffModel.Create(buff.BuffId, buff.SkillId, 0, (int)duration);
                                newCharacterBuff.SetBuffInfo(buff);

                                client.Tamer.BuffList.Buffs.Add(newCharacterBuff);

                                client.Send(new UpdateStatusPacket(client.Tamer));
                                client.Send(new AddBuffPacket(client.Tamer.GeneralHandler, buff, (short)0, duration)
                                    .Serialize());

                                await _sender.Send(new UpdateCharacterBuffListCommand(client.Tamer.BuffList));
                            }

                            // Verify if is Digimon Skill
                            if (buff.DigimonSkillCode > 0)
                            {
                                if (client.Partner.BuffList.Buffs.Any(x => x.BuffId == buff.BuffId))
                                {
                                    client.Send(new SystemMessagePacket($"Your Digimon already have this buff !!"));
                                    break;
                                }

                                var newDigimonBuff =
                                    DigimonBuffModel.Create(buff.BuffId, buff.SkillId, 0, (int)duration);
                                newDigimonBuff.SetBuffInfo(buff);

                                client.Partner.BuffList.Buffs.Add(newDigimonBuff);

                                client.Send(new UpdateStatusPacket(client.Tamer));
                                client.Send(new AddBuffPacket(client.Partner.GeneralHandler, buff, (short)0, duration)
                                    .Serialize());

                                await _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));
                            }

                            client.Send(new SystemMessagePacket($"New buff added"));
                        }
                        else if (command[1].ToLower() == "remove")
                        {
                            // Verify if is Tamer Skill
                            if (buff.SkillCode > 0)
                            {
                                var characterBuff =
                                    client.Tamer.BuffList.ActiveBuffs.FirstOrDefault(x => x.BuffId == buff.BuffId);

                                if (characterBuff == null)
                                {
                                    client.Send(new SystemMessagePacket($"CharacterBuff not found"));
                                    break;
                                }

                                client.Tamer.BuffList.Buffs.Remove(characterBuff);

                                client.Send(new UpdateStatusPacket(client.Tamer));
                                client.Send(new RemoveBuffPacket(client.Tamer.GeneralHandler, buff.BuffId).Serialize());

                                await _sender.Send(new UpdateCharacterBuffListCommand(client.Tamer.BuffList));
                            }

                            // Verify if is Digimon Skill
                            if (buff.DigimonSkillCode > 0)
                            {
                                var digimonBuff =
                                    client.Partner.BuffList.ActiveBuffs.FirstOrDefault(x => x.BuffId == buff.BuffId);

                                if (digimonBuff == null)
                                {
                                    client.Send(new SystemMessagePacket($"DigimonBuff not found"));
                                    break;
                                }

                                client.Partner.BuffList.Buffs.Remove(digimonBuff);

                                client.Send(new UpdateStatusPacket(client.Tamer));
                                client.Send(
                                    new RemoveBuffPacket(client.Partner.GeneralHandler, buff.BuffId).Serialize());

                                await _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));
                            }

                            client.Send(new SystemMessagePacket($"Buff removed !!"));
                        }
                        else
                        {
                            client.Send(new SystemMessagePacket($"Unknown command.\nType !buff (add/remove) (buffId)"));
                            break;
                        }
                    }
                    else
                    {
                        client.Send(new SystemMessagePacket($"Buff not found !!"));
                    }
                }
                    break;

                case "title":
                {
                    var regex = @"title\s(add|remove)\s\d+";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !title (add/remove)"));
                        break;
                    }

                    if (command.Length < 3)
                    {
                        client.Send(
                            new SystemMessagePacket("Invalid command format.\nType !title (add/remove) (titleId)"));
                        break;
                    }

                    if (!short.TryParse(command[2], out var titleId))
                    {
                        client.Send(new SystemMessagePacket("Invalid item ID format. Please provide a valid number."));
                        break;
                    }

                    if (command[1].ToLower() == "add")
                    {
                        var newTitle =
                            _assets.AchievementAssets.FirstOrDefault(x => x.QuestId == titleId && x.BuffId > 0);

                        if (newTitle != null)
                        {
                            var buff = _assets.BuffInfo.FirstOrDefault(x => x.BuffId == newTitle.BuffId);

                            var duration = UtilitiesFunctions.RemainingTimeSeconds(0);

                            var newDigimonBuff = DigimonBuffModel.Create(buff.BuffId, buff.SkillId);

                            newDigimonBuff.SetBuffInfo(buff);

                            foreach (var partner in client.Tamer.Digimons.Where(x => x.Id != client.Tamer.Partner.Id))
                            {
                                var partnernewDigimonBuff = DigimonBuffModel.Create(buff.BuffId, buff.SkillId);

                                partnernewDigimonBuff.SetBuffInfo(buff);

                                partner.BuffList.Add(partnernewDigimonBuff);

                                await _sender.Send(new UpdateDigimonBuffListCommand(partner.BuffList));
                            }

                            client.Partner.BuffList.Add(newDigimonBuff);

                            var mapClient = _mapServer.FindClientByTamerId(client.TamerId);

                            if (mapClient == null)
                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new AddBuffPacket(client.Partner.GeneralHandler, buff, (short)0, 0).Serialize());
                            else
                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new AddBuffPacket(client.Partner.GeneralHandler, buff, (short)0, 0).Serialize());

                            client.Tamer.UpdateCurrentTitle(titleId);

                            if (mapClient == null)
                                _dungeonServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new UpdateCurrentTitlePacket(client.Tamer.AppearenceHandler, titleId).Serialize());
                            else
                                _mapServer.BroadcastForTamerViewsAndSelf(client.TamerId,
                                    new UpdateCurrentTitlePacket(client.Tamer.AppearenceHandler, titleId).Serialize());

                            client.Send(new UpdateStatusPacket(client.Tamer));

                            await _sender.Send(new UpdateCharacterTitleCommand(client.TamerId, titleId));
                            await _sender.Send(new UpdateDigimonBuffListCommand(client.Partner.BuffList));
                        }
                        else
                        {
                            client.Send(new SystemMessagePacket($"Title {titleId} not found !!"));
                            break;
                        }
                    }
                    else if (command[1].ToLower() == "remove")
                    {
                        client.Send(new SystemMessagePacket($"Remove not implemented, sorry :)"));
                        break;
                    }
                }
                    break;

                #endregion

                // -- PARTY --------------------------------------

                #region Party

                case "party":
                {
                    //var regex = @"^party\s*$";
                    var regex = @"(party\slist$){1}|(party\sinfo$){1}";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !party"));
                        break;
                    }

                    if (command[1].ToLower() == "list")
                    {
                        var party = _partyManager.FindParty(client.TamerId);

                        if (party == null)
                        {
                            client.Send(new SystemMessagePacket($"The target tamer is not in a party."));
                        }
                        else
                        {
                            client.Send(new SystemMessagePacket($"Packet: Party Member List Sended !!"));
                            client.Send(new PartyMemberListPacket(party, client.TamerId,
                                (byte)(party.Members.Count - 1)));
                        }
                    }
                    else if (command[1].ToLower() == "info")
                    {
                        var party = _partyManager.FindParty(client.TamerId);

                        if (party == null)
                        {
                            client.Send(new SystemMessagePacket($"The target tamer is not in a party."));
                        }
                        else
                        {
                            client.Send(new SystemMessagePacket($"Packet: Party Member Info Sended !!"));
                            client.Send(new PartyMemberInfoPacket(party[client.TamerId]).Serialize());
                        }
                    }
                }
                    break;

                case "partymove":
                {
                    var regex = @"^partymove\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !party"));
                        break;
                    }

                    var party = _partyManager.FindParty(client.TamerId);

                    if (party == null)
                    {
                        client.Send(new SystemMessagePacket($"The target tamer is not in a party."));
                        _logger.Warning($"The target tamer  {client.Tamer.Name} is not in a party.");
                    }
                    else
                    {
                        foreach (var target in party.Members.Values.Where(x => x.Id != client.TamerId))
                        {
                            var targetClient = _mapServer.FindClientByTamerId(target.Id);

                            if (targetClient == null) targetClient = _dungeonServer.FindClientByTamerId(target.Id);

                            if (targetClient == null) continue;

                            client.Send(new SystemMessagePacket($"Updating your position for tamer {target.Name}."));

                            targetClient.Send(
                                UtilitiesFunctions.GroupPackets(
                                    new PartyMemberWarpGatePacket(party[client.TamerId], targetClient.Tamer)
                                        .Serialize(),
                                    new PartyMemberMovimentationPacket(party[client.TamerId]).Serialize()
                                )
                            );
                        }
                    }
                }
                    break;

                #endregion

                // -- PVP ----------------------------------------

                #region Pvp

                /*case "pvp":
                    {
                        var regex = @"(pvp\son){1}|(pvp\soff){1}";
                        var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                        if (!match.Success)
                        {
                            client.Send(new SystemMessagePacket($"Unknown command.\nType !pvp (on/off)"));
                            break;
                        }

                        if (client.Tamer.InBattle)
                        {
                            client.Send(new SystemMessagePacket($"You can't turn off pvp on battle !"));
                            break;
                        }

                        switch (command[1])
                        {
                            case "on":
                                {
                                    if (client.Tamer.PvpMap == false)
                                    {
                                        client.Tamer.PvpMap = true;
                                        client.Send(new NoticeMessagePacket($"PVP turned on !!"));
                                    }
                                    else client.Send(new NoticeMessagePacket($"PVP is already on ..."));
                                }
                                break;

                            case "off":
                                {
                                    if (client.Tamer.PvpMap == true)
                                    {
                                        client.Tamer.PvpMap = false;
                                        client.Send(new NoticeMessagePacket($"PVP turned off !!"));
                                    }
                                    else client.Send(new NoticeMessagePacket($"PVP is already off ..."));
                                }
                                break;
                        }
                    }
                    break;*/

                #endregion

                // -- Assets ----------------------------------------

                #region Reload Assets

                case "assetreload":
                {
                    var regex = @"^assetreload\s*$";
                    var match = Regex.Match(message, regex, RegexOptions.IgnoreCase);

                    if (!match.Success)
                    {
                        client.Send(new SystemMessagePacket($"Unknown command.\nType !assetreload"));
                        break;
                    }

                    var assetsLoader = _assets.Reload();
                }
                    break;

                #endregion

                // -- HELP ---------------------------------------

                #region Help

                case "help":
                {
                    var commandsList = new List<string>
                    {
                        "hatch",
                        "tamer",
                        "digimon",
                        "currency",
                        "reload",
                        "dc",
                        "ban",
                        "item",
                        "gfstorage",
                        "cashstorage",
                        "hide",
                        "show",
                        "inv",
                        "storage",
                        "godmode",
                        "unlockevos",
                        "openseals",
                        "summon",
                        "heal",
                        "stats",
                        "tools",
                        "fullacc",
                        "evopack",
                        "spacepack",
                        "clon",
                        "maptamers",
                        "updatestats",
                        "live",
                        "maintenance",
                        "notice",
                        "ann",
                        "where",
                        "tp",
                        "tpto",
                        "tptamer",
                        "exit",
                        "buff",
                        "title",
                        "party",
                        "partymove",
                        "assetreload",
                        "delete",
                    };

                    var packetsToSend = new List<SystemMessagePacket>
                        { new SystemMessagePacket($"SYSTEM COMMANDS:", ""), };

                    int count = 0;

                    foreach (var chunk in commandsList.Chunk(10))
                    {
                        string commandsString = "";
                        chunk.ToList().ForEach(x =>
                        {
                            count++;
                            var space = count > 9 ? "   " : "    ";
                            var name = $"{count}.{space}!{x}";
                            if (x != chunk.Last())
                            {
                                name += "\n";
                            }

                            commandsString += name;
                        });
                        packetsToSend.Add(new SystemMessagePacket(commandsString, ""));
                    }

                    // Convert packetsToSend to serialized form
                    var serializedPackets = packetsToSend.Select(x => x.Serialize()).ToArray();
                    client.Send(
                        UtilitiesFunctions.GroupPackets(
                            serializedPackets
                        ));
                }
                    break;

                #endregion

                // -----------------------------------------------

                default:
                    client.Send(
                        new SystemMessagePacket($"Unknown command.\nCheck the available commands typing !help"));
                    break;
            }
        }

        private async Task AddItemToInventory(GameClient client, int itemId, int amount)
        {
            var newItem = new ItemModel();
            newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == itemId));
            newItem.ItemId = itemId;
            newItem.Amount = amount;

            if (newItem.IsTemporary)
                newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

            if (client.Tamer.Inventory.AddItem(newItem))
            {
                client.Send(new ReceiveItemPacket(newItem, InventoryTypeEnum.Inventory));
                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            }
        }

        private async Task SummonMonster(GameClient client, SummonModel? SummonInfo)
        {
            foreach (var mobToAdd in SummonInfo.SummonedMobs)
            {
                var mob = (SummonMobModel)mobToAdd.Clone();

                int radius = 500;
                var random = new Random();

                int xOffset = random.Next(-radius, radius + 1);
                int yOffset = random.Next(-radius, radius + 1);

                int bossX = client.Tamer.Location.X + xOffset;
                int bossY = client.Tamer.Location.Y + yOffset;

                if (client.DungeonMap)
                {
                    var map = _dungeonServer.Maps.FirstOrDefault(
                        x => x.Clients.Exists(gameClient => gameClient.TamerId == client.TamerId));

                    var mobId = map.SummonMobs.Count + 1;

                    mob.SetId(mobId);

                    if (mob?.Location?.X != 0 && mob?.Location?.Y != 0)
                    {
                        bossX = mob.Location.X;
                        bossY = mob.Location.Y;

                        mob.SetLocation(client.Tamer.Location.MapId, bossX, bossY);
                    }
                    else
                    {
                        mob.SetLocation(client.Tamer.Location.MapId, bossX, bossY);
                    }

                    mob.SetDuration();
                    mob.SetTargetSummonHandle(client.Tamer.GeneralHandler);
                    _dungeonServer.AddSummonMobs(client.Tamer.Location.MapId, mob, client.TamerId);
                }
                else
                {
                    var map = _mapServer.Maps.FirstOrDefault(x =>
                        x.Clients.Exists(gameClient => gameClient.TamerId == client.TamerId));
                    var mobId = map.SummonMobs.Count + 1;

                    mob.SetId(mobId);

                    if (mob?.Location?.X != 0 && mob?.Location?.Y != 0)
                    {
                        bossX = mob.Location.X;
                        bossY = mob.Location.Y;

                        mob.SetLocation(client.Tamer.Location.MapId, bossX, bossY);
                    }
                    else
                    {
                        mob.SetLocation(client.Tamer.Location.MapId, bossX, bossY);
                    }

                    mob.SetDuration();
                    mob.SetTargetSummonHandle(client.Tamer.GeneralHandler);
                    _mapServer.AddSummonMobs(client.Tamer.Location.MapId, mob);
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}