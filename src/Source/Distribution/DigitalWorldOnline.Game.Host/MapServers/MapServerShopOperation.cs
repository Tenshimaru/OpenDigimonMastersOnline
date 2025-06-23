using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.Packets.PersonalShop;
using DigitalWorldOnline.Commons.Utils;

namespace DigitalWorldOnline.GameHost
{
    public sealed partial class MapServer
    {
        private void ShowOrHideConsignedShop(GameMap map, CharacterModel tamer)
        {
            var shopsToShow = map.ConsignedShops.Where(x => x.Channel == tamer.Channel).Except(map.ConsignedShopsToRemove).ToList();

            foreach (var consignedShop in shopsToShow)
            {
                var distanceDifference = UtilitiesFunctions.CalculateDistance(
                     tamer.Location.X, consignedShop.Location.X,
                     tamer.Location.Y, consignedShop.Location.Y);

                if (distanceDifference <= _startToSee)
                    ShowConsignedShop(map, consignedShop, tamer.Id);
                else if (distanceDifference >= _stopSeeing)
                    HideConsignedShop(map, consignedShop, tamer.Id);
            }

        }

        private void ShowConsignedShop(GameMap map, ConsignedShop shopToShow, long tamerToSeeId)
        {
            if (!map.ViewingConsignedShop(shopToShow.Id, tamerToSeeId))
            {
                _logger.Debug($"Loading consigned shop {shopToShow.Id} - {shopToShow.ShopName} for tamer {tamerToSeeId}...");
                map.ShowConsignedShop(shopToShow.Id, tamerToSeeId);

                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamerToSeeId);
                targetClient?.Send(new LoadConsignedShopPacket(shopToShow).Serialize());
            }
        }

        private void HideConsignedShop(GameMap map, ConsignedShop shopToHide, long tamerToBlindId)
        {
            if (map.ViewingConsignedShop(shopToHide.Id, tamerToBlindId))
            {
                _logger.Debug($"Hiding consigned shop {shopToHide.Id} - {shopToHide.ShopName} for tamer {tamerToBlindId}...");
                map.HideConsignedShop(shopToHide.Id, tamerToBlindId);

                var targetClient = map.Clients.FirstOrDefault(x => x.TamerId == tamerToBlindId);
                targetClient?.Send(new UnloadConsignedShopPacket(shopToHide).Serialize());
            }
        }
    }
}