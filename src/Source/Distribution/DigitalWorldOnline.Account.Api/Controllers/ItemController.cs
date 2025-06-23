using AutoMapper;
using DigitalWorldOnline.Api.Dtos.Converters;
using DigitalWorldOnline.Api.Dtos.In;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Models.Asset;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DigitalWorldOnline.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly Serilog.ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ItemController(
            ISender sender,
            Serilog.ILogger logger,
            IConfiguration configuration,
            IMapper mapper,
            IMemoryCache cache)
        {
            _sender = sender;
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
            _cache = cache;
        }


        [HttpPost]
        public async Task<IActionResult> ProcessItem(int AccountId, int itemId, int amount)
        {

            // Verifica se os detalhes do item estão em cache
            if (!_cache.TryGetValue($"ItemInfo_{itemId}", out IList<ItemAssetModel> itemInfo))
            {
                // Se não estiver em cache, busca os detalhes do item e adiciona ao cache
                itemInfo =  _mapper.Map<IList<ItemAssetModel>>(await _sender.Send(new ItemAssetsQuery()));
                _cache.Set($"ItemInfo_{itemId}", itemInfo, TimeSpan.FromMinutes(10)); // Define um tempo de expiração para o cache (por exemplo, 10 minutos)
            }

            var newItem = new ItemModel();
            newItem.SetItemInfo(itemInfo.FirstOrDefault(x => x.ItemId == itemId));

           

            newItem.ItemId = itemId;
            newItem.Amount = amount;

            if (newItem.IsTemporary)
                newItem.SetRemainingTime((uint)newItem.ItemInfo.UsageTimeMinutes);

            var itemClone = (ItemModel)newItem.Clone();
            var characterDto = await _sender.Send(new AccountByIdQuery(AccountId));

            var characterModel = _mapper.Map<ItemListModel>(characterDto.ItemList.FirstOrDefault(x => x.Type == Commons.Enums.ItemListEnum.CashWarehouse)); // Utilizando o mapper para realizar o mapeamento

            if (characterModel != null)
            {


                if (characterModel.AddItem(itemClone))
                {
                    await _sender.Send(new UpdateItemsCommand(characterModel));
                }
                else
                {
                    return Problem();
                }

            }


            // Retorna um OK como exemplo de sucesso
            return Ok(new { ItemId = itemId, Amount = amount, Message = "Item processed successfully" });
        }

    }
}
