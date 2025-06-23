using DigitalWorldOnline.Application.Separar.Commands.Update;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.PacketProcessor;
using DigitalWorldOnline.Commons.Interfaces;
using MediatR;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    public class ProgressUpdatePacketProcessor : IGamePacketProcessor
    {
        public GameServerPacketEnum Type => GameServerPacketEnum.ProgressUpdate;

        private readonly ISender _sender;
        private readonly ILogger _logger;

        public ProgressUpdatePacketProcessor(
            ISender sender,
            ILogger logger
        )
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Process(GameClient client, byte[] packetData)
        {
            var packet = new GamePacketReader(packetData);

            var objectId = packet.ReadInt();

            UpdateProgressValue(client, objectId);

            await _sender.Send(new UpdateCharacterProgressCompleteCommand(client.Tamer.Progress));
        }

        private void UpdateProgressValue(GameClient client, int questId)
        {
            UpdateQuestComplete(client, questId);
        }

        private void UpdateQuestComplete(GameClient client, int qIDX)
        {
            int intValue = GetBitValue(client.Tamer.Progress.CompletedDataValue, qIDX - 1);

            if (intValue == 0)
                SetBitValue(client.Tamer.Progress.CompletedDataValue, qIDX - 1, 1);
        }
        private int GetBitValue(int[] array, int x)
        {
            int arrIDX = x / 32;
            int bitPosition = x % 32;

            if (arrIDX >= array.Length)
            {
                _logger.Error($"Invalid array index for achievement {x}.");
                throw new ArgumentOutOfRangeException("Invalid array index");
            }

            int value = array[arrIDX];
            return (value >> bitPosition) & 1;
        }

        private void SetBitValue(int[] array, int x, int bitValue)
        {
            int arrIDX = x / 32;
            int bitPosition = x % 32;

            if (arrIDX >= array.Length)
            {
                _logger.Error($"Invalid array index on set bit value for achievement {x}.");
                throw new ArgumentOutOfRangeException("Invalid array index on set bit value.");
            }

            if (bitValue != 0 && bitValue != 1)
            {
                _logger.Error($"Invalid bit value. Only 0 or 1 are allowed for achievement {x}.");
                throw new ArgumentException("Invalid bit value. Only 0 or 1 are allowed.");
            }

            int value = array[arrIDX];
            int mask = 1 << bitPosition;

            if (bitValue == 1)
                array[arrIDX] = value | mask;
            else
                array[arrIDX] = value & ~mask;
        }

    }
}