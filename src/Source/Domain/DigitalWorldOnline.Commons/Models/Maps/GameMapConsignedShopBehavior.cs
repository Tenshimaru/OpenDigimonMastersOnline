using DigitalWorldOnline.Commons.Models.TamerShop;

namespace DigitalWorldOnline.Commons.Models.Map
{
    public sealed partial class GameMap
    {
        public bool ViewingConsignedShop(long consignedShopKey, long tamerTarget)
        {
            if (!ConsignedShopView.ContainsKey(consignedShopKey))
                ConsignedShopView.Add(consignedShopKey, new List<long>());

            return ConsignedShopView
                .FirstOrDefault(x => x.Key == consignedShopKey).Value?
                .Contains(tamerTarget) ??
                false;
        }

        public void ShowConsignedShop(long consignedShopKey, long tamerTarget)
        {
            ConsignedShopView
                .FirstOrDefault(x => x.Key == consignedShopKey).Value?
                .Add(tamerTarget);
        }

        public void HideConsignedShop(long consignedShopKey, long tamerTarget)
        {
            ConsignedShopView
                .FirstOrDefault(x => x.Key == consignedShopKey).Value?
                .Remove(tamerTarget);
        }

        public void UpdateConsignedShops(List<ConsignedShop> consignedShops)
        {
            ConsignedShopsToRemove.Clear();
            ConsignedShopsToRemove
                .AddRange(ConsignedShops
                .Except(consignedShops)
                .ToList());

            ConsignedShops.Clear();
            ConsignedShops = consignedShops;
        }
    }
}