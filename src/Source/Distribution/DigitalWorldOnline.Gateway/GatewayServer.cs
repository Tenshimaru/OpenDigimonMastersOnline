using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;
using DigitalWorldOnline.Gateway.Models;

namespace DigitalWorldOnline.Gateway
{
    public class GatewayServer : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly GatewayConfig _config;
        private TcpListener? _listener;

        public GatewayServer(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Bind configuration to GatewayConfig object
            _config = _configuration.GetSection("GatewayConfig").Get<GatewayConfig>()!;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Starting Gateway Service...");
            _listener = new TcpListener(IPAddress.Parse(_config.GatewayIP), _config.GatewayPort);
            _listener.Start();
            _logger.Information($"Gateway is running at {_config.GatewayIP}:{_config.GatewayPort}");

            // Start accepting clients in a background task
            Task.Run(() => AcceptClientsAsync(cancellationToken), cancellationToken);

            return Task.CompletedTask;
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await _listener!.AcceptTcpClientAsync(cancellationToken);
                _logger.Information("Client connected.");

                _ = Task.Run(() => HandleClientAsync(client, cancellationToken), cancellationToken);
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8);
            writer.AutoFlush = true;

            _logger.Information("Handling a new connection.");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Step 1: Read a message from the client (which is the app name + command)
                    string? receivedMessage = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(receivedMessage))
                    {
                        _logger.Information("Client disconnected.");
                        break;
                    }

                    _logger.Information("Received message from client: {Message}", receivedMessage);

                    // Assuming the message can have two parts delimited by a separator (e.g., "|")
                    // Format: "AppName|CommandToSendToApp"
                    var parts = receivedMessage.Split('|', 2);

                    if (parts.Length < 2)
                    {
                        _logger.Warning("Invalid message format received from client: {Message}", receivedMessage);
                        await writer.WriteLineAsync("Error: Invalid message format. Expected 'AppName|Command'.");
                        continue; // Continue listening for the next message
                    }

                    string appName = parts[0];
                    string commandToSend = parts[1];

                    // Step 2: Look up the app configuration
                    var appConfig = _config.Applications.FirstOrDefault(a =>
                        a.Name.Equals(appName, StringComparison.OrdinalIgnoreCase));
                    if (appConfig == null)
                    {
                        _logger.Warning("Requested unknown application: {AppName}", appName);
                        await writer.WriteLineAsync("Error: Application not found.");
                        continue; // Continue listening for the next message
                    }

                    _logger.Information("Connecting to application: {AppName} at {IP}:{Port}...", appConfig.Name,
                        appConfig.IP, appConfig.Port);

                    // Step 3: Connect to the target app and forward the command
                    using TcpClient appClient = new TcpClient();
                    await appClient.ConnectAsync(appConfig.IP, appConfig.Port, cancellationToken);

                    using NetworkStream appStream = appClient.GetStream();

                    // Forward the command to the application
                    byte[] data = Encoding.UTF8.GetBytes(commandToSend);
                    await appStream.WriteAsync(data, 0, data.Length, cancellationToken);

                    _logger.Information("Command forwarded to application: {Command}", commandToSend);

                    // Step 4: Read response from the application
                    byte[] buffer = new byte[1024];
                    int bytesRead = await appStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    string appResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    _logger.Information("Response from application {AppName}: {Response}", appConfig.Name, appResponse);

                    // Forward the response back to the original client
                    await writer.WriteLineAsync(appResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while handling the client.");
                await writer.WriteLineAsync($"Error: {ex.Message}");
            }
            finally
            {
                _logger.Information("Client connection closed.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Stopping Gateway Service...");
            _listener?.Stop();
            return Task.CompletedTask;
        }
    }
}