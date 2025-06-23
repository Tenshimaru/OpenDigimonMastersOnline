using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Application.Separar.Queries;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Character;
using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Models.Summon;
using DigitalWorldOnline.Commons.Packets.Chat;
using DigitalWorldOnline.Commons.Packets.GameServer;
using DigitalWorldOnline.Commons.Packets.Items;
using DigitalWorldOnline.Commons.Packets.MapServer;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Game.Managers;
using DigitalWorldOnline.GameHost;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Diagnostics.Eventing.Reader;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class TamerChargeXCrystalPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TamerChargeXCrystal;

        private readonly MapServer _mapServer;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public TamerChargeXCrystalPacketProcessor(
            MapServer mapServer,
            ISender sender,
            ILogger logger)
        {
            _mapServer = mapServer;
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            if (client.Tamer.XGauge >= 500)
            {
                client.Tamer.ConsumeXg(500);
                client.Tamer.SetXCrystals(1);
            }
            else
            {
                _logger.Verbose($"You don't have enought Gauge to store !!");
            }

            /*if (client.Tamer.ConsumeXg(500))
                client.Tamer.SetXCrystals(1);
            else
                _logger.Information($"You don't have enought Gauge to store !!");*/

            client.Send(new TamerXaiResourcesPacket(client.Tamer.XGauge, client.Tamer.XCrystals));
        }
    }
}