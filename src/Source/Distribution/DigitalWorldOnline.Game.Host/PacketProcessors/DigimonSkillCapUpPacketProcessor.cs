using Microsoft.Extensions.Logging;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;
using DigitalWorldOnline.Application;
using System.Linq.Dynamic.Core;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Packets.GameServer;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class DigmonSkillCapUpPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.SkillCapIncrease;

        private readonly Dictionary<int, List<int>> ValidTypesBySection;

        private readonly AssetsLoader _assetsLoader;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public DigmonSkillCapUpPacketProcessor(AssetsLoader assetsloader, ISender sender)
        {
            _assetsLoader = assetsloader;
            _sender = sender;

            ValidTypesBySection = new Dictionary<int, List<int>> {
                // Rookie - Mega (Lv15, 20, 25)
                { 20201, new List<int> { 3, 4, 5, 6 } },
                { 20202, new List<int> { 3, 4, 5, 6 } },
                { 20203, new List<int> { 3, 4, 5, 6 } },
                // Rookie X - Mega X (Lv15, 20, 25)
                { 20206, new List<int> { 11, 12, 13, 14 } },
                { 20207, new List<int> { 11, 12, 13, 14 } },
                { 20208, new List<int> { 11, 12, 13, 14 } },
                // Burst Mode (Lv15, 20, 25)
                { 20211, new List<int> { 7 } },
                { 20212, new List<int> { 7 } },
                { 20213, new List<int> { 7 } },
                // Burst Mode X (Lv15, 20, 25)
                { 20216, new List<int> { 15 } },
                { 20217, new List<int> { 15 } },
                { 20218, new List<int> { 15 } },
                // Jogress (Lv15, 20, 25)
                { 20221, new List<int> { 8 } },
                { 20222, new List<int> { 8 } },
                { 20223, new List<int> { 8 } },
                // Jogress X (Lv15, 20, 25)
                { 20226, new List<int> { 16 } },
                { 20227, new List<int> { 16 } },
                { 20228, new List<int> { 16 } },
                // Spirit (Lv15, 20, 25)
                { 20231, new List<int> { 9 } },
                { 20232, new List<int> { 9 } },
                { 20233, new List<int> { 9 } },
                // Hybrid (Lv15, 20, 25)
                { 20236, new List<int> { 10 } },
                { 20237, new List<int> { 10 } },
                { 20238, new List<int> { 10 } },
                // Extra (Lv15, 20, 25)
                { 20241, new List<int> { 17 } },
                { 20242, new List<int> { 17 } },
                { 20243, new List<int> { 17 } },
            };
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var invSlot = packet.ReadUInt(); // The slot where the item was used
            var itemId = packet.ReadUInt(); // The ID of the used item
            var formSlot = packet.ReadUInt(); // The slot of the current active Evolution(digimon) - 1-indexed (so 1 for rookie/base, 2 for Champion/First Evolution, etc)

            // Gets a reference for the current digimon
            var dEvo = client.Partner.CurrentEvolution;

            if (dEvo == null)
                return;

            // Gets a reference to the item itself(the digicode used)
            var digiCode = client.Tamer.Inventory.FindItemBySlot((int)invSlot);

            // Slot was empty or the item type is not valid(not a DigiCode)
            if (digiCode?.ItemInfo?.Type != 202)
            {
                return;
            }

            var result = GetSkillCapIncreaseResult(digiCode.ItemInfo.Section,
                        _assetsLoader.DigimonBaseInfo.FirstOrDefault(d => d.Type == dEvo.Type)?.EvolutionType ?? -1,
                        dEvo.Skills[0].MaxLevel);

            if (result != DigimonSkillCapIncreaseResultEnum.Success)
            {
                client.Send(new DigimonSkillCapUpResultPacket(result, formSlot, dEvo, invSlot, itemId));
                return;
            }

            // Increases the level of all skills by 5
            foreach (var sk in dEvo.Skills)
                sk.IncreaseSkillStep();

            // Remove the used item
            client.Tamer.Inventory.RemoveOrReduceItem(digiCode, 1);

            // Updates the Database
            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateEvolutionCommand(client.Partner.CurrentEvolution));

            // Notifies the Client of the updated information
            client.Send(UtilitiesFunctions.GroupPackets(
                        new ItemConsumeSuccessPacket(client.Tamer.GeneralHandler, (short)invSlot).Serialize(),
                        new DigimonSkillCapUpResultPacket(result, formSlot, dEvo, invSlot, itemId).Serialize(),
                        new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()
                        ));
        }

        private DigimonSkillCapIncreaseResultEnum GetSkillCapIncreaseResult(int section, int evoStage, int currentLevel)
        {

            int remainder = section % 10;
            int targetLevel = -1;

            if (remainder == 1 || remainder == 6)
                targetLevel = 15;
            else if (remainder == 2 || remainder == 7)
                targetLevel = 20;
            else if (remainder == 3 || remainder == 8)
                targetLevel = 25;

            Console.WriteLine(targetLevel.ToString());

            if (targetLevel == -1)
                return DigimonSkillCapIncreaseResultEnum.ItemTypeError;
            if (!ValidTypesBySection.TryGetValue(section, out var validEvos))
                return DigimonSkillCapIncreaseResultEnum.ItemTypeError;

            if (!validEvos.Contains(evoStage))
                return DigimonSkillCapIncreaseResultEnum.ItemTypeError;

            if (currentLevel >= targetLevel)
                return DigimonSkillCapIncreaseResultEnum.AlreadyOpen;

            if ((targetLevel == 15 && currentLevel != 10) ||
                (targetLevel == 20 && currentLevel != 15) ||
                (targetLevel == 25 && currentLevel != 20))
                return DigimonSkillCapIncreaseResultEnum.SkipBeforeLevel;

            return DigimonSkillCapIncreaseResultEnum.Success;
        }
    }
}