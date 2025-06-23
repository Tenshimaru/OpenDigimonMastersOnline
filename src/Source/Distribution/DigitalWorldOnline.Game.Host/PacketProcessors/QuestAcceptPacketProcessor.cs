using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.Items;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class QuestAcceptPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.QuestAccept;

        private readonly AssetsLoader _assets;
        private readonly ILogger _logger;
        private readonly ISender _sender;

        public QuestAcceptPacketProcessor(AssetsLoader assets, ILogger logger, ISender sender)
        {
            _assets = assets;
            _logger = logger;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var questId = packet.ReadShort();

            _logger.Debug($"Quest id: {questId}");

            if (client.Tamer.Progress.AcceptQuest(questId))
            {
                var questInfo = _assets.Quest.FirstOrDefault(x => x.QuestId == questId);

                if (questInfo == null)
                {
                    _logger.Error($"Unknown quest id {questId}.");
                    client.Send(new SystemMessagePacket($"Unknown quest id {questId}."));
                    client.Tamer.Progress.RemoveQuest(questId);
                    return;
                }

                foreach (var questSupply in questInfo.QuestSupplies)
                {
                    var item = new ItemModel();
                    item.SetItemId(questSupply.ItemId);
                    item.SetAmount(questSupply.Amount);
                    item.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == item.ItemId));

                    if (item.ItemInfo == null)
                    {
                        _logger.Error($"Item information not found for item {item.ItemId}.");
                        client.Send(new SystemMessagePacket($"Item information not found for item {item.ItemId}."));
                        client.Tamer.Progress.RemoveQuest(questId);
                        return;
                    }

                    var itemClone = (ItemModel)item.Clone();

                    if (!client.Tamer.Inventory.AddItem(itemClone))
                    {
                        client.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                        client.Tamer.Progress.RemoveQuest(questId);
                        return;
                    }             

                }

                _logger.Debug($"Tamer {client.TamerId}:{client.Tamer.Name} accepted quest {questId}.");

                foreach (var questSupply in questInfo.QuestSupplies)
                {
                    _logger.Verbose($"Character {client.TamerId} received quest {questId} supply item {questSupply.ItemId} x{questSupply.Amount}.");
                }
         
                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                await _sender.Send(new AddCharacterProgressCommand(client.Tamer.Progress));
            }
        }
    }
}
