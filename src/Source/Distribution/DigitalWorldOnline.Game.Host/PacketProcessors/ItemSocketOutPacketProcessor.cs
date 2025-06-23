using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;


namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ItemSocketOutPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ItemSocketOut;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly AssetsLoader _assets;
        public ItemSocketOutPacketProcessor(
            ILogger logger,
            ISender sender,
            AssetsLoader assetsLoader)
        {
            _logger = logger;
            _sender = sender;
            _assets = assetsLoader;

        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            _ = packet.ReadInt();
            var vipEnabled = packet.ReadByte();
            int npcId = packet.ReadInt();
            short sourceSlot = packet.ReadShort();
            short destinationSlot = packet.ReadShort();
            byte OrderSlot = packet.ReadByte();

            var itemInfo = client.Tamer.Inventory.FindItemBySlot(sourceSlot);
         
            if (itemInfo != null)
            {
                var avaliableSocket = itemInfo.SocketStatus.First(x => x.Slot == OrderSlot);
              
                if (avaliableSocket != null)
                {
                    var newItem = new ItemModel();
                    newItem.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == avaliableSocket.AttributeId));



                    newItem.ItemId = avaliableSocket.AttributeId;
                    newItem.Amount = 1;

                    if (newItem.IsTemporary)
                        newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

                    newItem.SetPower(itemInfo.Power);
                    newItem.SetReroll(0);

                    var newAvaliableStatus = newItem.AccessoryStatus.First(x => x.Value == 0);

                    newAvaliableStatus.SetType(avaliableSocket.Type);
                    newAvaliableStatus.SetValue(avaliableSocket.Value);

                    var itemClone = (ItemModel)newItem.Clone();

                    if(client.Tamer.Inventory.AddItem(itemClone))
                    {
                        await _sender.Send(new UpdateItemSocketStatusCommand(itemClone));
                        await _sender.Send(new UpdateItemAccessoryStatusCommand(itemClone));
                    }
                    else
                    {
                        client.Send(new PickItemFailPacket(PickItemFailReasonEnum.InventoryFull));
                    }
                  
                    avaliableSocket.SetType(0);
                    avaliableSocket.SetAttributeId(0);
                    avaliableSocket.SetValue(0);
                  
                }


                client.Tamer.Inventory.RemoveBits(itemInfo.ItemInfo.ScanPrice / 2 * 3 + 1);

                client.Send(new ItemSocketOutPacket((int)client.Tamer.Inventory.Bits));

                await _sender.Send(new UpdateItemSocketStatusCommand(itemInfo));
                await _sender.Send(new UpdateItemAccessoryStatusCommand(itemInfo));

                await _sender.Send(new UpdateItemsCommand(client.Tamer.Inventory));
                await _sender.Send(new UpdateItemListBitsCommand(client.Tamer.Inventory));
            }
        }
    }
}