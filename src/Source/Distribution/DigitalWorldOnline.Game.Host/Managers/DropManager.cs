using DigitalWorldOnline.Commons.Models;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Map;
using DigitalWorldOnline.Commons.Utils;
using Serilog;

namespace DigitalWorldOnline.Game.Managers
{
    public class DropManager
    {
        private long _dropId;
        private readonly ILogger _logger;

        public DropManager(ILogger logger)
        {
            _logger = logger;
            _dropId = UtilitiesFunctions.RandomInt();
        }

        public Drop CreateBitDrop(
            long ownerId,
            ushort ownerHandler,
            int minAmount,
            int maxAmount,
            short mapId,
            int x,
            int y
        )
        {
            var drop = new Drop(
                _dropId,
                ownerId,
                ownerHandler,
                new ItemModel(
                    90600,
                    UtilitiesFunctions.RandomInt(
                        minAmount,
                        maxAmount
                    )
                ),
                new Location(
                    mapId,
                    x + UtilitiesFunctions.RandomInt(-100, 100),//TODO: externalizar
                    y + UtilitiesFunctions.RandomInt(-25, 25)
                )
            );

            _dropId++;

            _logger.Verbose($"Character {ownerId} dropped {drop.DropInfo.Amount} bits at map {drop.Location.MapId} X{drop.Location.X} Y{drop.Location.Y}.");

            return drop;
        }

        public Drop CreateItemDrop(
            long ownerId,
            ushort ownerHandler,
            int itemId,
            int minAmount,
            int maxAmount,
            short mapId,
            int x,
            int y,
            bool thrown = false
        )
        {
            var drop = new Drop(
                _dropId,
                ownerId,
                ownerHandler,
                new ItemModel (
                    itemId,
                    UtilitiesFunctions.RandomInt(
                        minAmount,
                        maxAmount
                    )
                ),
                new Location(
                    mapId,
                    x + UtilitiesFunctions.RandomInt(-100, 100),
                    y + UtilitiesFunctions.RandomInt(-25, 25)
                ),
                thrown
            );

            _dropId++;

            if (thrown)
                _logger.Verbose($"Character {ownerId} throw away {drop.DropInfo.ItemId} x{drop.DropInfo.Amount} at map {drop.Location.MapId} X{drop.Location.X} Y{drop.Location.Y}.");
            else
                _logger.Verbose($"Character {ownerId} dropped {drop.DropInfo.ItemId} x{drop.DropInfo.Amount} at map {drop.Location.MapId} X{drop.Location.X} Y{drop.Location.Y}.");

            return drop;
        }
    }
}
