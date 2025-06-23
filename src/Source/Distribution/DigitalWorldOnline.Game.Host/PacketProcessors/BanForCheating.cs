using System;
using DigitalWorldOnline.Application.Separar.Commands.Create;
using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.Packets.GameServer;
using MediatR;
using Microsoft.Data.SqlClient;
using Serilog;

namespace DigitalWorldOnline.Game.PacketProcessors
{
    internal class BanForCheating
    {
        public ILogger _logger;
        public ISender _sender;

        public BanForCheating(ILogger logger, ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        public async void BanAccountForCheating(long accountId, AccountBlockEnum type, string reason,
            DateTime startDate,
            DateTime endDate)
        {
            try
            {
                await _sender.Send(new AddAccountBlockCommand(accountId, type, reason, startDate, endDate));
                _logger.Verbose($"Account ID: {accountId} has been banned");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to ban account id {accountId} and error:\n " + ex.Message, accountId);
            }
        }

        public string SimpleBan(long accountId, string Name, AccountBlockEnum type, string reason,
            GameClient? client = null, string? banMessage = null)
        {
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(3);
            BanAccountForCheating(accountId, type, reason, startDate, endDate);

            // Retorna a mensagem de banimento
            if (client != null)
            {
                TimeSpan timeRemaining = endDate - startDate;

                uint secondsRemaining = (uint)timeRemaining.TotalSeconds;
                client.Send(new BanUserPacket(secondsRemaining, banMessage ?? reason));
            }

            return $"User {Name} has been banned for 3 days! reason: {reason}.";
        }

        // Método simplificado para usar no código de banimento
        public string BanAccountWithMessage(long accountId, string Name, AccountBlockEnum type, string reason,
            GameClient? client = null, string? banMessage = null)
        {
            // Chama o método BanAccountForCheating
            /*
             Para banir por 1 hora a partir da data atual:   DateTime.Now.AddHours(1)
             Para banir por 1 dia a partir da data atual:    DateTime.Now.AddDays(1)
             Para banir por 1 semana a partir da data atual: DateTime.Now.AddDays(7)
             Para banir por 1 mês a partir da data atual:    DateTime.Now.AddMonths(1)
             Para banir por 1 ano a partir da data atual:    DateTime.Now.AddYears(1)
             Para banir permanentemente : DateTime.MaxValue

             fazer um sistema decrescente a cada ban

               DateTime endDate;

                // Definir a data de término do banimento com base no tipo de duração
                switch (durationType)
                {
                    case "1Hour":
                        endDate = DateTime.Now.AddHours(1);
                        break;
                    case "1Day":
                        endDate = DateTime.Now.AddDays(1);
                        break;
                    case "1Week":
                        endDate = DateTime.Now.AddDays(7);
                        break;
                    case "1Month":
                        endDate = DateTime.Now.AddMonths(1);
                        break;
                    case "1Year":
                        endDate = DateTime.Now.AddYears(1);
                        break;
                    default:
                        endDate = DateTime.Now;  // Se nenhum tipo for válido, banir por 0 dias (não recomendado)
                        break;
                }
             */
            var startDate = DateTime.Now;
            var endDate = DateTime.MaxValue;
            BanAccountForCheating(accountId, type, reason, startDate, endDate);
            // DateTime.Now.AddDays(1) = 1 day | DateTime.MaxValue = Permanent
            // Retorna a mensagem de banimento
            if (client != null)
            {
                TimeSpan timeRemaining = endDate - startDate;

                uint secondsRemaining = (uint)timeRemaining.TotalSeconds;
                client.Send(new BanUserPacket(secondsRemaining, banMessage ?? reason));
            }

            return $"User {Name} has been banned permanently for {reason}.";
        }
    }
}