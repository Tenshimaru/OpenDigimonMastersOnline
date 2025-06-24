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

        private readonly AssetsLoader _assetsLoader;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public DigmonSkillCapUpPacketProcessor(AssetsLoader assetsloader, ISender sender)
        {
            _assetsLoader = assetsloader;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var invSlot = packet.ReadUInt(); // The slot where the item was used
            var itemId = packet.ReadUInt(); // The ID of the used item
            var formSlot = packet.ReadUInt(); // The slot of the current active Evolution(digimon) - 1-indexed (so 1 for rookie/base, 2 for Champion/First Evolution, etc)

            var dEvo = client.Partner.Evolutions[(int)formSlot - 1]; // Tries to get a reference to the current evolution from the current digimon

            if (dEvo == null)
                return;

            var digiCode = client.Tamer.Inventory.FindItemBySlot((int)invSlot); // Tries to get a reference to the item itself(the digicode used)

            // TODO: Check if the item used is a Digicode, for now people can inject a packet and consume any item to increase the level.
            if (digiCode?.ItemId == 0)
            {
                // Slot was emtpy,or the it had an ID of 0(invalid item)
                client.Send(new DigimonSkillCapUpResultPacket(Commons.Enums.ClientEnums.DigimonSkillCapIncreaseResultEnum.ItemTypeError, formSlot, dEvo, invSlot, itemId));
                return;
            }

            // Gets all "basic" skill for the current evolution
            var skillMappings = _assetsLoader.DigimonSkillInfo
                .Where(ds => ds.Type == dEvo.Type)
                .OrderBy(ds => ds.Slot)
                .ToArray();

            // Tries leveling all the basic skills by 5 levels
            // TODO: Check if the used Digicode is valid for the current skill Max Level
            var skillChanged = dEvo.Skills.Zip(skillMappings, (skill, mapping) =>
            {
                var skillInfo = _assetsLoader.SkillInfo.FirstOrDefault(si => si.SkillId == mapping.SkillId);
                if (skillInfo is null || skill.MaxLevel >= skillInfo.MaxLevel) return false;

                return skill.IncreaseSkillCap();
            }).Any(changed => changed);

            if (!skillChanged) // If the skill levels where not increased, no update need to be done.
                return;

            client.Tamer.Inventory.RemoveOrReduceItem(digiCode, 1); // Remove the used item

            // Updates the Database
            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateEvolutionCommand(client.Partner.CurrentEvolution));

            // Notifies the Client of the updated information
            // TODO: Maybe this need to be broadcasted to mapserver/dungeonserver, instead of just the client that used it?
            client.Send(UtilitiesFunctions.GroupPackets(new DigimonSkillCapUpResultPacket(Commons.Enums.ClientEnums.DigimonSkillCapIncreaseResultEnum.Success, formSlot, dEvo, invSlot, itemId).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize(),
                            new ItemConsumeSuccessPacket(client.Tamer.GeneralHandler, (short)invSlot).Serialize()));
        }
    }
}