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

            var invSlot = packet.ReadUInt();
            var itemId = packet.ReadUInt();
            var formSlot = packet.ReadUInt();

            var dEvo = client.Partner.Evolutions[(int)formSlot - 1];

            if (dEvo == null)
                return;

            var cSkCode = client.Tamer.Inventory.FindItemBySlot((int)invSlot);

            if (cSkCode?.ItemId == 0)
            {
                client.Send(new DigimonSkillCapUpResultPacket(Commons.Enums.ClientEnums.DigimonSkillCapIncreaseResultEnum.ItemTypeError, formSlot, dEvo, invSlot, itemId));
                return;
            }

            var skillMappings = _assetsLoader.DigimonSkillInfo
                .Where(ds => ds.Type == dEvo.Type)
                .OrderBy(ds => ds.Slot)
                .ToArray();

            var skillChanged = dEvo.Skills.Zip(skillMappings, (skill, mapping) =>
            {
                var skillInfo = _assetsLoader.SkillInfo.FirstOrDefault(si => si.SkillId == mapping.SkillId);
                if (skillInfo is null || skill.MaxLevel >= skillInfo.MaxLevel) return false;

                skill.IncreaseSkillCap();

                return true;
            }).Any(changed => changed);

            if (!skillChanged)
                return;

            client.Tamer.Inventory.RemoveOrReduceItem(cSkCode, 1);

            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateEvolutionCommand(client.Partner.CurrentEvolution));

            client.Send(UtilitiesFunctions.GroupPackets(new DigimonSkillCapUpResultPacket(Commons.Enums.ClientEnums.DigimonSkillCapIncreaseResultEnum.Success, formSlot, dEvo, invSlot, itemId).Serialize(),
                            new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize(),
                            new ItemConsumeSuccessPacket(client.Tamer.GeneralHandler, (short)invSlot).Serialize()));
        }
    }
}