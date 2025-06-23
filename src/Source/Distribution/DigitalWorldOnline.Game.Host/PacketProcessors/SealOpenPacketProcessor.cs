using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class SealOpenPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.OpenSeal;

        private readonly AssetsLoader _assets;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public SealOpenPacketProcessor(
            AssetsLoader assets,
            ISender sender,
            ILogger logger)
        {
            _assets = assets;
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var sealItem = client.Tamer.Inventory.FindItemBySlot(packet.ReadShort());
            var requestOpener = 1;
            var cardsRemain = 0;

            var sealId = sealItem.ItemId;

            if (sealItem.Amount >= 50)
            {
                requestOpener = (int)Math.Round((decimal)(sealItem.Amount / 50));
                cardsRemain = (short)Math.Round((decimal)(sealItem.Amount % 50));
            }

            var cardsToOpen = (short)(sealItem.Amount - cardsRemain);

            var availableOpenners = _assets.ItemInfo.Where(x => x.Type == 191 && x.Section == 19101);

            var opennersList = new List<ItemModel>();
            foreach (var availableOpenner in availableOpenners)
            {
                var inventoryCloser = client.Tamer.Inventory.FindItemsById(availableOpenner.ItemId);
                if (inventoryCloser != null) opennersList.AddRange(inventoryCloser);
            }

            opennersList = opennersList.OrderBy(x => x.Slot).ToList();

            var needOpeners = requestOpener;

            foreach (var openner in opennersList)
            {
                if (openner.Amount >= needOpeners)
                {
                    openner.ReduceAmount(needOpeners);
                    needOpeners = 0;
                }
                else
                {
                    needOpeners -= openner.Amount;
                    openner.SetAmount();
                }

                if (needOpeners == 0)
                    break;
            }

            if (needOpeners > 0)
            {
                _logger.Error($"Invalid openners amount for tamer {client.TamerId}.");
                client.Send(new SystemMessagePacket($"Invalid openners amount, reload your character."));
                client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));
                return;
            }

            var sealInfo = _assets.SealInfo.FirstOrDefault(x => x.SealId == sealId);
            if (sealInfo != null)
            {
                client.Tamer.SealList.AddOrUpdateSeal(sealId, cardsToOpen, sealInfo.SequentialId);
                client.Partner?.SetSealStatus(_assets.SealInfo);

                client.Send(new UpdateStatusPacket(client.Tamer));

                sealItem.SetAmount(cardsRemain);
                client.Tamer.Inventory.CheckEmptyItems();

                _logger.Verbose($"Character {client.TamerId} openned seal {sealInfo.SequentialId} x{cardsToOpen}.");

                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                await _sender.Send(new UpdateCharacterSealsCommand(client.Tamer.SealList));
            }
            else
            {
                _logger.Error($"Invalid seal asset for seal id {sealId} on tamer {client.TamerId} open seal.");
                client.Send(new SystemMessagePacket($"Invalid seal id {sealId}."));
                client.Send(new LoadInventoryPacket(client.Tamer.Inventory, InventoryTypeEnum.Inventory));
            }
        }
    }
}