using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.PersonalShop;
using DigitalWorldOnline.GameHost;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class PersonalShopOpenPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TamerShopList;

        private readonly AssetsLoader _assets;
        private readonly MapServer _mapServer;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ISender _sender;

        public PersonalShopOpenPacketProcessor(
            MapServer mapServer,
            AssetsLoader assets,
            ILogger logger,
            IMapper mapper,
            ISender sender)
        {
            _mapServer = mapServer;
            _assets = assets;
            _logger = logger;
            _mapper = mapper;
            _sender = sender;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            //_logger.Information($"--- PersonalShop View Packet 1515 ---");

            packet.Skip(4);
            var handler = packet.ReadInt();

            _logger.Debug($"Searching personal shop with handler {handler}...");
            var PersonalShop = _mapServer.FindClientByTamerHandle(handler);

            if (PersonalShop != null)
            {
                _logger.Debug($"Find Tamer {PersonalShop.Tamer.Name} with shop {PersonalShop.Tamer.ShopName} - {handler}.");

                foreach (var item in PersonalShop.Tamer.TamerShop.Items)
                {
                    item.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == item.ItemId));

                    //TODO: generalizar isso em rotina
                    if (item.ItemId > 0 && item.ItemInfo == null)
                    {
                        item.SetItemId();
                        PersonalShop.Tamer.TamerShop.CheckEmptyItems();
                        _logger.Debug($"Updating consigned shop item list...");
                        await _sender.Send(new UpdateItemsCommand(PersonalShop.Tamer.TamerShop));
                    }
                }


                _logger.Debug($"Sending consigned shop item list view packet...");
                client.Send(new PersonalShopItemsViewPacket(PersonalShop.Tamer.TamerShop, PersonalShop.Tamer.ShopName));
            }
            else
            {
                _logger.Debug($"Searching consigned shop with handler {handler}...");
                var consignedShop = _mapper.Map<ConsignedShop>(await _sender.Send(new ConsignedShopByHandlerQuery(handler)));

                if (consignedShop != null)
                {
                    var seller = _mapper.Map<CharacterModel>(await _sender.Send(new CharacterAndItemsByIdQuery(consignedShop.CharacterId)));

                    int count = seller.ConsignedShopItems.EquippedItems.GroupBy(x => x.ItemId).ToList().Count;

                    if (seller != null)
                    {
                        foreach (var item in seller.ConsignedShopItems.EquippedItems)
                        {
                            item.SetItemInfo(_assets.ItemInfo.FirstOrDefault(x => x.ItemId == item.ItemId));
  
                            if (item.ItemId > 0 && item.ItemInfo == null)
                            {
                                item.SetItemId();

                                PersonalShop.Tamer.TamerShop.CheckEmptyItems();

                                await _sender.Send(new UpdateItemsCommand(seller.ConsignedShopItems));
                            }
                           
                        }
                    }

                    if (seller.Name == client.Tamer.Name)
                    {
                        _logger.Debug($"Sending consigned shop item list view packet for owner");
                        client.Send(new ConsignedShopItemsViewPacket(consignedShop, seller.ConsignedShopItems, seller.Name, true));
                    }
                    else
                    {
                        _logger.Debug($"Sending consigned shop item list view packet...");
                        client.Send(new ConsignedShopItemsViewPacket(consignedShop, seller.ConsignedShopItems, seller.Name));
                    }
                }
                else
                {
                    _logger.Error($"Personal or Consigned Shop not found !!");
                }
            }     
        }
    }
}