using System.Globalization;
using DigitalWorldOnline.Commons.Utils;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using DigitalWorldOnline.Gateway.Models;

namespace DigitalWorldOnline.Gateway
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Console.Title = $"DMO - Gateway Server";
            try
            {
                var host = Host.CreateDefaultBuilder(args)
                    .UseSerilog()
                    .UseEnvironment("Development")
                    .ConfigureServices((context, services) =>
                    {
                        // Bind configuration to GatewayConfig
                        services.Configure<GatewayConfig>(context.Configuration.GetSection("GatewayConfig"));
                        
                        services.AddSingleton(ConfigureLogger(context.Configuration));

                        // Register GatewayServer
                        services.AddHostedService<GatewayServer>();
                        
                        services.AddTransient<Mediator>();
                    })
                    .ConfigureHostConfiguration(hostConfig =>
                    {
                        hostConfig.SetBasePath(Directory.GetCurrentDirectory())
                            .AddEnvironmentVariables(Constants.Configuration.EnvironmentPrefix)
                            .AddUserSecrets<Program>();
                    })
                    .Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The application failed to start.");
            }
            finally
            {
                 await Log.CloseAndFlushAsync();
            }
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(((Exception)e.ExceptionObject).InnerException);
            if (e.IsTerminating)
            {
                var message = "";
                var exceptionStackTrace = "";
                if (e.ExceptionObject is Exception exception)
                {
                    message = exception.Message;
                    exceptionStackTrace = exception.StackTrace;
                }

                Console.WriteLine($"{message}");
                Console.WriteLine($"{exceptionStackTrace}");
                Console.WriteLine("Terminating by unhandled exception...");
            }
            else
                Console.WriteLine("Received unhandled exception.");

            Console.ReadLine();
        }

        private static ILogger ConfigureLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Verbose)
                    .WriteTo.File(
                        path: configuration["Log:VerboseRepository"] ?? @"logs\Verbose\GatewayServer-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 10))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug)
                    .WriteTo.File(
                        path: configuration["Log:DebugRepository"] ?? @"logs\Debug\GatewayServer-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 10))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
                    .WriteTo.File(
                        path: configuration["Log:InformationRepository"] ?? @"logs\Information\GatewayServer-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 10))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                    .WriteTo.File(
                        path: configuration["Log:WarningRepository"] ?? @"logs\Warning\GatewayServer-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 10))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                    .WriteTo.File(
                        path: configuration["Log:ErrorRepository"] ?? @"logs\Error\GatewayServer-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 10))
                .CreateLogger();
        }
    }
}