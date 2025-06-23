using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using DigitalWorldOnline.GameHost.EventsServer;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class HatchSpiritEvolutionPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.HatchSpiritEvolution;

        private readonly StatusManager _statusManager;
        private readonly MapServer _mapServer;
        private readonly DungeonsServer _dungeonServer;
        private readonly EventServer _eventServer;
        private readonly PvpServer _pvpServer;
        private readonly AssetsLoader _assets;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public HatchSpiritEvolutionPacketProcessor(
            StatusManager statusManager,
            MapServer mapServer,
            DungeonsServer dungeonsServer,
            EventServer eventServer,
            PvpServer pvpServer,
            AssetsLoader assets,
            ILogger logger,
            ISender sender
        )
        {
            _statusManager = statusManager;
            _mapServer = mapServer;
            _dungeonServer = dungeonsServer;
            _eventServer = eventServer;
            _pvpServer = pvpServer;
            _assets = assets;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var targetType = packet.ReadInt();
            var digiName = packet.ReadString();
            var x = packet.ReadByte();
            var NpcId = packet.ReadInt();

            var extraEvolutionNpc = _assets.ExtraEvolutions.FirstOrDefault(extraEvolutionNpcAssetModel =>
                extraEvolutionNpcAssetModel.NpcId == NpcId);

            if (extraEvolutionNpc == null)
            {
                _logger.Warning($"Extra Evolution NPC not found for Hatch !!");
                return;
            }

            var extraEvolutionInfo = extraEvolutionNpc.ExtraEvolutionInformation
                .FirstOrDefault(extraEvolutionInformationAssetModel =>
                    extraEvolutionInformationAssetModel.ExtraEvolution.Any(extraEvolutionAssetModel =>
                        extraEvolutionAssetModel.DigimonId == targetType))?.ExtraEvolution;

            if (extraEvolutionInfo == null)
            {
                _logger.Warning($"extraEvolutionInfo == null");
                return;
            }

            var extraEvolution = extraEvolutionInfo.FirstOrDefault(extraEvolutionAssetModel =>
                extraEvolutionAssetModel.DigimonId == targetType);

            if (extraEvolution == null)
            {
                _logger.Warning($"extraEvolution == null");
                return;
            }

            if (!client.Tamer.Inventory.RemoveBits(extraEvolution.Price))
            {
                client.Send(new SystemMessagePacket($"Insuficient bits !!"));
                _logger.Warning($"Insuficient bits for hatch in NPC Id: {NpcId} for tamer {client.TamerId}.");
                return;
            }

            var materialToPacket = new List<ExtraEvolutionMaterialAssetModel>();
            var requiredsToPacket = new List<ExtraEvolutionRequiredAssetModel>();

            foreach (var material in extraEvolution.Materials)
            {
                var itemToRemove = client.Tamer.Inventory.FindItemById(material.ItemId);

                if (itemToRemove != null)
                {
                    materialToPacket.Add(material);
                    client.Tamer.Inventory.RemoveOrReduceItemWithoutSlot(
                        new ItemModel(material.ItemId, material.Amount));

                    break;
                }
            }

            foreach (var material in extraEvolution.Requireds)
            {
                var itemToRemove = client.Tamer.Inventory.FindItemById(material.ItemId);

                if (itemToRemove != null)
                {
                    requiredsToPacket.Add(material);
                    client.Tamer.Inventory.RemoveOrReduceItemWithoutSlot(
                        new ItemModel(material.ItemId, material.Amount));

                    if (extraEvolution.Requireds.Count <= 3)
                    {
                        break;
                    }
                }
            }

            /*byte i = 0;
            while (i < client.Tamer.DigimonSlots)
            {
                if (client.Tamer.Digimons.FirstOrDefault(digimonModel => digimonModel.Slot == i) == null)
                    break;

                i++;
            }*/

            byte? digimonSlot = (byte)Enumerable.Range(0, client.Tamer.DigimonSlots)
                            .FirstOrDefault(slot => client.Tamer.Digimons.FirstOrDefault(x => x.Slot == slot) == null);

            if (digimonSlot == null)
                return;

            var newDigimon = DigimonModel.Create(
                digiName,
                targetType,
                targetType,
                DigimonHatchGradeEnum.Default,
                UtilitiesFunctions.GetLevelSize(3),
                (byte)digimonSlot
            );

            newDigimon.NewLocation(client.Tamer.Location.MapId, client.Tamer.Location.X, client.Tamer.Location.Y);

            newDigimon.SetBaseInfo(_statusManager.GetDigimonBaseInfo(newDigimon.BaseType));

            newDigimon.SetBaseStatus(
                _statusManager.GetDigimonBaseStatus(newDigimon.BaseType, newDigimon.Level, newDigimon.Size));

            newDigimon.AddEvolutions(_assets.EvolutionInfo.First(evolutionAssetModel =>
                evolutionAssetModel.Type == newDigimon.BaseType));

            if (newDigimon.BaseInfo == null || newDigimon.BaseStatus == null || !newDigimon.Evolutions.Any())
            {
                _logger.Warning($"Unknown digimon info for {newDigimon.BaseType}.");
                client.Send(new SystemMessagePacket($"Unknown digimon info for {newDigimon.BaseType}."));
                return;
            }

            newDigimon.SetTamer(client.Tamer);

            client.Tamer.AddDigimon(newDigimon);

            if (client.Tamer.Incubator.PerfectSize(newDigimon.HatchGrade, newDigimon.Size))
            {
                _mapServer.BroadcastGlobal(new NeonMessagePacket(NeonMessageTypeEnum.Scale, client.Tamer.Name,
                    newDigimon.BaseType, newDigimon.Size).Serialize());
                _dungeonServer.BroadcastGlobal(new NeonMessagePacket(NeonMessageTypeEnum.Scale, client.Tamer.Name,
                    newDigimon.BaseType, newDigimon.Size).Serialize());
                _eventServer.BroadcastGlobal(new NeonMessagePacket(NeonMessageTypeEnum.Scale, client.Tamer.Name,
                    newDigimon.BaseType, newDigimon.Size).Serialize());
                _pvpServer.BroadcastGlobal(new NeonMessagePacket(NeonMessageTypeEnum.Scale, client.Tamer.Name,
                    newDigimon.BaseType, newDigimon.Size).Serialize());
            }

            var digimonInfo = await _sender.Send(new CreateDigimonCommand(newDigimon));

            client.Send(new HatchFinishPacket(newDigimon, (ushort)(client.Partner.GeneralHandler + 1000), (byte)digimonSlot));

            client.Send(new HatchSpiritEvolutionPacket(targetType, (int)client.Tamer.Inventory.Bits, materialToPacket, requiredsToPacket));
            client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));

            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory.Id, client.Tamer.Inventory.Bits));

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

            _logger.Verbose(
                $"Character {client.TamerId} hatched spirit {newDigimon.Id}({newDigimon.BaseType}) with grade {newDigimon.HatchGrade} and size {newDigimon.Size}.");
        }
    }
}