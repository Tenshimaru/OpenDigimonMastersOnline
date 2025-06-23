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
    public class TamerConsumeXCrystalPacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.TamerConsumeXCrystal;

        private readonly MapServer _mapServer;
        private readonly ISender _sender;
        private readonly ILogger _logger;

        public TamerConsumeXCrystalPacketProcessor(MapServer mapServer, ISender sender, ILogger logger)
        {
            _mapServer = mapServer;
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            _logger.Debug($"XCrystal Consume Packet\n");

            _logger.Debug($"Tamer Gauge: {client.Tamer.XGauge} | Tamer XCrystal: {client.Tamer.XCrystals}");

            if ((client.Tamer.XGauge + 500) > client.Tamer.Xai.XGauge)
            {
                _logger.Debug($"Gauge on limit, Sending Packet 16033");
                client.Send(new XaiInfoPacket(client.Tamer.Xai));   // Send Packet 16033
            }
            else
            {
                client.Tamer.ConsumeXCrystal(1);
                client.Tamer.SetXGauge(500);
            }

            _logger.Debug($"Tamer Gauge: {client.Tamer.XGauge} | Tamer XCrystal: {client.Tamer.XCrystals}");
            client.Send(new TamerXaiResourcesPacket(client.Tamer.XGauge, client.Tamer.XCrystals));   // Send Packet 16032
        }

    }
}