using MediatR;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Commons.Enums.Admin;

namespace DigitalWorldOnline.Admin.Pages
{
    public partial class Index
    {
        [Inject]
        public ISender Sender { get; set; }

        private bool Loading = false;

        // Dashboard Data
        private int TotalAccounts = 0;
        private int TotalPlayers = 0;
        private int TotalEvents = 0;
        private int TotalServers = 0;

        protected override async Task OnInitializedAsync()
        {
            Loading = true;
            await LoadDashboardData();
            Loading = false;
        }

        private async Task LoadDashboardData()
        {
            try
            {
                // Get total accounts
                var accountsResult = await Sender.Send(new GetAccountsQuery(0, 1, "", SortDirectionEnum.Asc, ""));
                TotalAccounts = accountsResult.TotalRegisters;

                // Get total players
                var playersResult = await Sender.Send(new GetPlayersQuery(0, 1, "", SortDirectionEnum.Asc, ""));
                TotalPlayers = playersResult.TotalRegisters;

                // Get total events
                var eventsResult = await Sender.Send(new GetEventsQuery(0, 1, "", SortDirectionEnum.Asc, ""));
                TotalEvents = eventsResult.TotalRegisters;

                // Get total servers
                var serversResult = await Sender.Send(new GetServersQuery(0, 1, "", SortDirectionEnum.Asc, ""));
                TotalServers = serversResult.TotalRegisters;
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                Console.WriteLine($"Error loading dashboard data: {ex.Message}");
            }
        }

        private async Task Teste1()
        {
            // Cria um novo IHostBuilder
            //var hostBuilder = CreateHostBuilder(args);
            //
            //// Constr�i o IHost
            //var host = hostBuilder.Build();
            //
            //// Adiciona um m�todo de callback para o evento ApplicationStopping
            //host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping.Register(OnApplicationStopping);
            //
            //// Inicia a execu��o ass�ncrona do IHost
            //await host.RunAsync();
            //
            //ProcessStartInfo startInfo = new ProcessStartInfo
            //{
            //    FileName = "D:\\Projetos\\DWO\\dso-project\\src\\Source\\Distribution\\DigitalWorldOnline.Account.Host\\bin\\Debug\\net6.0\\DigitalWorldOnline.Account.exe",
            //    RedirectStandardOutput = false,
            //    UseShellExecute = false,
            //    CreateNoWindow = true
            //};
            //
            //Process process = new Process
            //{
            //    StartInfo = startInfo
            //};
            //
            //process.EnableRaisingEvents = true;
            //
            //process.Exited += ProcessExited;
            //
            //process.Start();
            //
            //_accountProcessId = process.Id;
        }

        private static void ProcessExited(object sender, EventArgs e)
        {
            Console.WriteLine("Processo encerrado.");
        }

        private void Teste2()
        {
            //var accountProcess = Process.GetProcessById(_accountProcessId);
            //
            //accountProcess?.Kill();
        }
    }
}
