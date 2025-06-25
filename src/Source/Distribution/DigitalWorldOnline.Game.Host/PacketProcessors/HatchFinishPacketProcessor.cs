using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.DTOs.Digimon;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;
using System.Collections.Concurrent;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class HatchFinishPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.HatchFinish;

        private readonly StatusManager _statusManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly AssetsLoader _assets;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        // Thread-safe dictionary for hatch locks per player
        private static readonly ConcurrentDictionary<long, SemaphoreSlim> _hatchLocks = new();

        public HatchFinishPacketProcessor(StatusManager statusManager, AssetsLoader assets,
            MapServer mapServer, DungeonsServer dungeonsServer, EventServer eventServer, PvpServer pvpServer,
            ILogger logger, ISender sender, IMapper mapper)
        {
            _statusManager = statusManager;
            _assets = assets;
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var hatchLock = _hatchLocks.GetOrAdd(client.TamerId, _ => new SemaphoreSlim(1, 1));

            try
            {
                await hatchLock.WaitAsync();

                var packet = new GamePacketReader(packetData);

                packet.Skip(5);
                var digiName = packet.ReadString();

                // Validações básicas
                if (string.IsNullOrWhiteSpace(digiName) || digiName.Length > 12)
                {
                    _logger.Warning($"[HATCH] {client.Tamer.Name} attempted to hatch with invalid name: '{digiName}'");
                    client.Send(new SystemMessagePacket("Invalid Digimon name."));
                    return;
                }

                // Check if there's an egg in the incubator
                if (client.Tamer.Incubator.EggId == 0)
                {
                    _logger.Warning($"[HATCH] {client.Tamer.Name} attempted to hatch without an egg in incubator");
                    client.Send(new SystemMessagePacket("No egg in incubator."));
                    return;
                }

                var hatchInfo = _assets.Hatchs.FirstOrDefault(x => x.ItemId == client.Tamer.Incubator.EggId);

                if (hatchInfo == null)
                {
                    _logger.Error($"[HATCH] Unknown hatch info for egg {client.Tamer.Incubator.EggId} for {client.Tamer.Name}");
                    client.Send(new SystemMessagePacket($"Unknown hatch info for egg {client.Tamer.Incubator.EggId}."));
                    return;
                }

                // Check if egg is ready to hatch
                if (client.Tamer.Incubator.HatchLevel < 3) // Assuming level 3 is minimum to hatch
                {
                    _logger.Warning($"[HATCH] {client.Tamer.Name} attempted to hatch egg at insufficient level {client.Tamer.Incubator.HatchLevel}");
                    client.Send(new SystemMessagePacket("Egg is not ready to hatch yet."));
                    return;
                }

                // Buscar slot disponível de forma thread-safe
                byte? digimonSlot = FindAvailableDigimonSlot(client);

                if (digimonSlot == null)
                {
                    _logger.Warning($"[HATCH] {client.Tamer.Name} attempted to hatch but has no available Digimon slots");
                    client.Send(new SystemMessagePacket("No available Digimon slots."));
                    return;
                }

                await ProcessHatching(client, digiName, hatchInfo, (byte)digimonSlot);
            }
            finally
            {
                hatchLock.Release();

                // Clean up old locks to prevent memory leaks
                if (_hatchLocks.Count > 1000)
                {
                    var keysToRemove = _hatchLocks.Keys.Take(100).ToList();
                    foreach (var key in keysToRemove)
                    {
                        if (_hatchLocks.TryRemove(key, out var lockToDispose))
                        {
                            lockToDispose.Dispose();
                        }
                    }
                }
            }
        }

        private byte? FindAvailableDigimonSlot(GameClient client)
        {
            for (byte i = 0; i < client.Tamer.DigimonSlots; i++)
            {
                if (client.Tamer.Digimons.FirstOrDefault(x => x.Slot == i) == null)
                {
                    return i;
                }
            }
            return null;
        }

        private async Task ProcessHatching(GameClient client, string digiName, HatchAssetModel hatchInfo, byte digimonSlot)
        {
            try
            {

                var newDigimon = DigimonModel.Create(
                    digiName,
                    hatchInfo.HatchType,
                    hatchInfo.HatchType,
                    (DigimonHatchGradeEnum)client.Tamer.Incubator.HatchLevel,
                    client.Tamer.Incubator.GetLevelSize(),
                    digimonSlot
                );

                newDigimon.NewLocation(
                    client.Tamer.Location.MapId,
                    client.Tamer.Location.X,
                    client.Tamer.Location.Y
                );

                newDigimon.SetBaseInfo(
                    _statusManager.GetDigimonBaseInfo(
                        newDigimon.BaseType
                    )
                );

                newDigimon.SetBaseStatus(
                    _statusManager.GetDigimonBaseStatus(
                        newDigimon.BaseType,
                        newDigimon.Level,
                        newDigimon.Size
                    )
                );

                var digimonEvolutionInfo = _assets.EvolutionInfo.FirstOrDefault(x => x.Type == newDigimon.BaseType);

                if (digimonEvolutionInfo == null)
                {
                    _logger.Error($"[HATCH] No evolution info found for Digimon type {newDigimon.BaseType}");
                    client.Send(new SystemMessagePacket($"Evolution info not found for this Digimon type."));
                    return;
                }

                newDigimon.AddEvolutions(digimonEvolutionInfo);

                if (newDigimon.BaseInfo == null || newDigimon.BaseStatus == null || !newDigimon.Evolutions.Any())
                {
                    _logger.Error($"[HATCH] Incomplete digimon info for {newDigimon.BaseType} - BaseInfo: {newDigimon.BaseInfo != null}, BaseStatus: {newDigimon.BaseStatus != null}, Evolutions: {newDigimon.Evolutions.Any()}");
                    client.Send(new SystemMessagePacket($"Incomplete digimon information for {newDigimon.BaseType}."));
                    return;
                }

                newDigimon.SetTamer(client.Tamer);

                // Check again if slot is still available before creating in database
                if (client.Tamer.Digimons.Any(x => x.Slot == digimonSlot))
                {
                    _logger.Error($"[HATCH] Slot {digimonSlot} is no longer available for {client.Tamer.Name}");
                    client.Send(new SystemMessagePacket("Digimon slot is no longer available."));
                    return;
                }

                // Criar Digimon no banco de dados
                var digimonInfo = _mapper.Map<DigimonModel>(await _sender.Send(new CreateDigimonCommand(newDigimon)));

                if (digimonInfo == null)
                {
                    _logger.Error($"[HATCH] Failed to create Digimon in database for {client.Tamer.Name}");
                    client.Send(new SystemMessagePacket("Failed to create Digimon. Please try again."));
                    return;
                }

                // Add to player and remove egg from incubator
                client.Tamer.AddDigimon(digimonInfo);
                client.Tamer.Incubator.RemoveEgg();
                await _sender.Send(new UpdateIncubatorCommand(client.Tamer.Incubator));

                // Check if it's perfect size for broadcast
                if (client.Tamer.Incubator.PerfectSize(newDigimon.HatchGrade, newDigimon.Size))
                {
                    client.SendToAll(new NeonMessagePacket(NeonMessageTypeEnum.Scale, client.Tamer.Name,
                        newDigimon.BaseType, newDigimon.Size).Serialize());
                }

                client.Send(new HatchFinishPacket(newDigimon, (ushort)(client.Partner.GeneralHandler + 1000), digimonSlot));

                _logger.Information($"[HATCH_SUCCESS] {client.Tamer.Name} successfully hatched {digiName} (Type: {newDigimon.BaseType}, Grade: {newDigimon.HatchGrade}, Size: {newDigimon.Size}) in slot {digimonSlot}");

                await ProcessDigimonEvolutions(client, newDigimon, digimonInfo, digimonEvolutionInfo);
                await ProcessEncyclopedia(client, newDigimon, digimonEvolutionInfo);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[HATCH_ERROR] Error processing hatch for {client.Tamer.Name}");
                client.Send(new SystemMessagePacket("An error occurred while hatching. Please try again."));
            }
        }

        private async Task ProcessDigimonEvolutions(GameClient client, DigimonModel newDigimon, DigimonModel digimonInfo, EvolutionAssetModel digimonEvolutionInfo)
        {
            try
            {

                if (digimonInfo != null)
                {
                    newDigimon.SetId(digimonInfo.Id);
                    var slot = -1;

                    foreach (var digimon in newDigimon.Evolutions)
                    {
                        slot++;

                        if (slot < digimonInfo.Evolutions.Count)
                        {
                            var evolution = digimonInfo.Evolutions[slot];

                            if (evolution != null)
                            {
                                digimon.SetId(evolution.Id);

                                var skillSlot = -1;

                                foreach (var skill in digimon.Skills)
                                {
                                    skillSlot++;

                                    if (skillSlot < evolution.Skills.Count)
                                    {
                                        var dtoSkill = evolution.Skills[skillSlot];
                                        skill.SetId(dtoSkill.Id);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[HATCH_EVOLUTION_ERROR] Error processing evolutions for {client.Tamer.Name}");
            }
        }

        private async Task ProcessEncyclopedia(GameClient client, DigimonModel newDigimon, EvolutionAssetModel digimonEvolutionInfo)
        {
            try
            {
                var encyclopediaExists = client.Tamer.Encyclopedia.Exists(x => x.DigimonEvolutionId == digimonEvolutionInfo?.Id);

                if (encyclopediaExists)
                {
                    _logger.Debug($"[HATCH_ENCYCLOPEDIA] Encyclopedia entry already exists for type {newDigimon.BaseType}");
                }
                else
                {
                    if (digimonEvolutionInfo != null)
                    {
                        var encyclopedia = CharacterEncyclopediaModel.Create(client.TamerId, digimonEvolutionInfo.Id,
                            newDigimon.Level, newDigimon.Size, 0, 0, 0, 0, 0, false, false);

                        newDigimon.Evolutions?.ForEach(x =>
                        {
                            var evolutionLine = digimonEvolutionInfo.Lines.FirstOrDefault(y => y.Type == x.Type);
                            byte slotLevel = 0;
                            if (evolutionLine != null)
                            {
                                slotLevel = evolutionLine.SlotLevel;
                            }

                            encyclopedia.Evolutions.Add(CharacterEncyclopediaEvolutionsModel.Create(encyclopedia.Id, x.Type,
                                slotLevel, Convert.ToBoolean(x.Unlocked)));
                        });

                        var encyclopediaAdded = await _sender.Send(new CreateCharacterEncyclopediaCommand(encyclopedia));

                        if (encyclopediaAdded != null)
                        {
                            client.Tamer.Encyclopedia.Add(encyclopediaAdded);
                            _logger.Information($"[HATCH_ENCYCLOPEDIA] Added encyclopedia entry for {newDigimon.BaseType} to {client.Tamer.Name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[HATCH_ENCYCLOPEDIA_ERROR] Error processing encyclopedia for {client.Tamer.Name}");
            }
        }
    }
}