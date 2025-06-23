using AutoMapper;
using DigitalWorldOnline.Application;
using DigitalWorldOnline.Application.Routines.Commands;
using DigitalWorldOnline.Application.Routines.Queries;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Models.DTOs.Routine;
using MediatR;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DigitalWorldOnline.Routine
{
    public sealed class RoutineServer : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly AssetsLoader _assets;
        private readonly ILogger _logger;
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public RoutineServer(IHostApplicationLifetime hostApplicationLifetime,
            AssetsLoader assets,
            ILogger logger,
            ISender sender,
            IMapper mapper)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _assets = assets.Load();
            _logger = logger;
            _sender = sender;
            _mapper = mapper;
        }

        /// <summary>
        /// The default hosted service "starting" method.
        /// </summary>
        /// <param name="cancellationToken">Control token for the operation</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information($"Starting {GetType().Name}...");

            Console.Title = $"DMO - {GetType().Name}";

            _hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
            _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
            _hostApplicationLifetime.ApplicationStopped.Register(OnStopped);

            while (_assets.Loading) await Task.Delay(1000, cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var routines = _mapper.Map<List<RoutineModel>>(await _sender.Send(new GetActiveRoutinesQuery(), cancellationToken));

                foreach (var routine in routines.Where(x => x.ExecutionTime))
                {
                    _logger.Information($"Executing {routine.Name}...");
                    switch (routine.Type)
                    {
                        case RoutineTypeEnum.DailyQuests:
                            await _sender.Send(new ExecuteDailyQuestsRoutineCommand(_assets.DailyQuestList), cancellationToken);
                            break;

                        case RoutineTypeEnum.DailyColiseum:
                        case RoutineTypeEnum.WeeklyColiseum:
                        case RoutineTypeEnum.MonthlyColiseum:
                        case RoutineTypeEnum.SeasonalColiseum:
                        case RoutineTypeEnum.DailySpinMachine:
                        case RoutineTypeEnum.WeeklySpinMachine:
                        case RoutineTypeEnum.EventRanking:
                        case RoutineTypeEnum.PvpRanking:
                            _logger.Error($"Routine not implemented {routine.Name}.");
                            break;
                    }

                    _logger.Information($"Updating {routine.Name} execution time...");
                    await Task.Delay(60000, cancellationToken);
                    await _sender.Send(new UpdateRoutineExecutionTimeCommand(routine.Id), cancellationToken);
                }

                _logger.Information($"Waiting routines execution time...");
                await Task.Delay(300000, cancellationToken);
            }
        }

        /// <summary>
        /// The default hosted service "stopping" method
        /// </summary>
        /// <param name="cancellationToken">Control token for the operation</param>
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// The default hosted service "started" method action
        /// </summary>
        private void OnStarted()
        {
            _logger.Information($"{GetType().Name} started.");
        }

        /// <summary>
        /// The default hosted service "stopping" method action
        /// </summary>
        private void OnStopping()
        {
            _logger.Information($"Stopping {GetType().Name}...");
        }

        /// <summary>
        /// The default hosted service "stopped" method action
        /// </summary>
        private void OnStopped()
        {
            _logger.Information($"{GetType().Name} stopped.");
        }
    }
}
