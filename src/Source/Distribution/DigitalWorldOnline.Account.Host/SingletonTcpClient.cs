using System.Net.Sockets;
using System.Text;

namespace DigitalWorldOnline.Account
{
    public class SingletonTcpClient
    {
        private static SingletonTcpClient? _instance; // Holds the singleton instance
        private static readonly object _lock = new(); // For thread safety

        // TcpClient and Streams
        private TcpClient? _tcpClient;
        private StreamReader? _reader;
        private StreamWriter? _writer;

        private string _serverIp;
        private int _serverPort;

        // Private constructor (Singleton pattern)
        private SingletonTcpClient(string serverIp, int serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
        }

        /// <summary>
        /// Gets the singleton instance of the TcpClient.
        /// </summary>
        public static SingletonTcpClient GetInstance(string serverIp, int serverPort)
        {
            lock (_lock) // Ensure thread safety
            {
                if (_instance == null)
                {
                    _instance = new SingletonTcpClient(serverIp, serverPort);
                }

                return _instance;
            }
        }

        /// <summary>
        /// Establishes a connection if not already connected.
        /// </summary>
        public async Task ConnectAsync()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                return; // Already connected
            }

            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_serverIp, _serverPort);

            var stream = _tcpClient.GetStream();
            _reader = new StreamReader(stream, Encoding.UTF8);
            _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            Console.WriteLine($"Connected to the server at {_serverIp}:{_serverPort}");
        }

        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        public async Task SendMessageAsync(string message)
        {
            if (_writer == null)
            {
                throw new InvalidOperationException("Not connected to the server.");
            }

            await _writer.WriteLineAsync(message);
        }

        /// <summary>
        /// Reads a response from the server.
        /// </summary>
        public async Task<string?> ReadResponseAsync()
        {
            if (_reader == null)
            {
                throw new InvalidOperationException("Not connected to the server.");
            }

            return await _reader.ReadLineAsync();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Disconnect()
        {
            _reader?.Dispose();
            _writer?.Dispose();
            _tcpClient?.Close();
            _tcpClient = null;

            Console.WriteLine("Disconnected from the server.");
        }
    }
}