using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ItemCraftPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ItemCraft;

        private readonly AssetsLoader _assets;
        private readonly ISender _sender;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ItemCraftPacketProcessor(
            AssetsLoader assets,
            ISender sender,
            ILogger logger,
            IMapper mapper)
        {
            _assets = assets;
            _sender = sender;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var npcId = packet.ReadInt();
            var sequencialId = packet.ReadInt();
            var requestAmount = packet.ReadInt();

            var increaseRateItem = packet.ReadInt(); //Não implementado no client
            var protectItem = packet.ReadInt(); //TODO: obter o item id correto para remoção

            var craftRecipe = _mapper.Map<ItemCraftAssetModel>(await _sender.Send(new ItemCraftAssetsByFilterQuery(npcId, sequencialId)));

            if (craftRecipe == null)
            {
                client.Send(new SystemMessagePacket($"Item craft not found with NPC id {npcId} and id {sequencialId}."));
                _logger.Warning($"Item craft not found with NPC id {npcId} and id {sequencialId} for tamer {client.TamerId}.");
                return;
            }

            var totalPrice = requestAmount * craftRecipe.Price;

            /*if (!client.Tamer.Inventory.RemoveBits(totalPrice))
            {
                client.Send(new SystemMessagePacket($"Insuficient bits for item craft NPC id {npcId} and id {sequencialId}."));
                _logger.Warning($"Insuficient bits for item craft NPC id {npcId} and id {sequencialId} for tamer {client.TamerId}.");
                return;
            }*/

            var craftedItem = new ItemModel(craftRecipe.ItemId, craftRecipe.Amount);
            craftedItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == craftRecipe.ItemId));

            craftedItem.SetDefaultRemainingTime();

            var totalCrafted = 0;
            var tries = requestAmount;

            while (tries > 0)
            {
                foreach (var material in craftRecipe.Materials)
                {
                    if (!client.Tamer.Inventory.RemoveOrReduceItemWithoutSlot(new ItemModel(material.ItemId, material.Amount)))
                    {
                        client.Disconnect();
                        return;
                    }
                }

                if (!client.Tamer.Inventory.RemoveBits(craftRecipe.Price))
                {
                    client.Send(new SystemMessagePacket($"Insuficient bits for item craft NPC id {npcId} and id {sequencialId}."));
                    return;
                }

                if (craftRecipe.SuccessRate >= UtilitiesFunctions.RandomByte(maxValue: 100))
                {
                    var tempItem = (ItemModel)craftedItem.Clone();
                    client.Tamer.Inventory.AddItem(tempItem);

                    totalCrafted++;
                }

                tries--;
            }

            var materialList = string.Join(',', craftRecipe.Materials.Select(x => $"{x.ItemId} x{x.Amount}"));

            _logger.Verbose($"Character {client.TamerId} crafted {craftRecipe.ItemId} with {materialList} and {craftRecipe.Price} bits {requestAmount} times.");

            await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));
            await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));

            client.Send(
                UtilitiesFunctions.GroupPackets(
                    new CraftItemPacket(craftRecipe, requestAmount, totalCrafted).Serialize(),
                    new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory).Serialize()
                )
            );
        }
    }
}