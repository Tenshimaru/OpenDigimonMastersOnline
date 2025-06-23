using System.Net;
using System.Net.Sockets;

namespace DigitalWorldOnline.Commons.Entities
{
    public abstract class GameServer : IDisposable
    {
        public const string AuthenticationServerAddress = "AuthenticationServer:Address";
        public const string AuthenticationServerPort = "AuthenticationServer:Port";
        public const string AuthenticationServerBacklog = "AuthenticationServer:Backlog";

        public const string CharacterServerAddress = "CharacterServer:Address";
        public const string CharacterServerPort = "CharacterServer:Port";
        public const string CharacterServerBacklog = "CharacterServer:Backlog";

        public const string GameServerAddress = "GameServer:Address";
        public const string GamerServerPublic = "GameServer:PublicAddress";
        public const string GameServerPort = "GameServer:Port";
        public const string GameServerBacklog = "GameServer:Backlog";

        public ManualResetEvent ResetEvent;
        public List<GameClient> Clients;
        public Socket? ServerListener;
        public TcpListener TcpListener;

        public event ClientDataReceiveEventHandler? DataReceived;
        public event ClientDataSendEventHandler? DataSent;
        public event ClientEventHandler? OnDisconnect;
        public event ClientEventHandler? OnConnect;

        public delegate void ClientDataReceiveEventHandler(object sender, GameClientEvent e, byte[] data);
        public delegate void ClientDataSendEventHandler(object sender, GameClientEvent e);
        public delegate void ClientEventHandler(object sender, GameClientEvent e);

        private readonly object ClientLock;

        private const string NullObjectExceptionMessage = "Received null object";

        public bool IsListening => ServerListener != null && ServerListener.IsBound;

        public Dictionary<string, List<DateTime>> ConnectionOccurrences;
        public List<string> RefusedAddresses;

        protected GameServer()
        {
            ResetEvent = new ManualResetEvent(true);
            ClientLock = new object();
            Clients = new();
            ConnectionOccurrences = new();
            RefusedAddresses = new();
        }

        public virtual void InitializeServer()
        {
            ServerListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerListener.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
        }

        public virtual bool Listen(string address, string port, string backlog = "5")
        {
            InitializeServer();

            if (IsListening)
            {
                throw new InvalidOperationException($"Server already running on {address} : {port}.");
            }

            try
            {
                IPAddress ipAddress = IPAddress.Parse(address);

                // Utiliza Any para escutar em todas as interfaces de rede
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, int.Parse(port));
                ServerListener?.Bind(localEndPoint);
            }
            catch (SocketException)
            {
                Shutdown();
                return false;
            }

            ServerListener?.Listen(int.Parse(backlog));

            ServerListener?.BeginAccept(AcceptCallback, null);

            return true;
        }


        private void AcceptCallback(IAsyncResult result)
        {
            if (!IsListening)
                return;

            try
            {
                Socket socket = ServerListener!.EndAccept(result);

                GameClient client = new(this, socket);

                Clients.Add(client);

                OnClientConnection(new GameClientEvent(client));

                client.BeginReceive(ReceiveCallback, client);
                ServerListener.BeginAccept(AcceptCallback, null);
            }
            catch { }
        }

        protected bool InvalidConnection(string? clientIpAddress)
        {
            if (string.IsNullOrEmpty(clientIpAddress))
                return true;

            if (RefusedAddresses.Contains(clientIpAddress))
                return true;

            if (!IPAddress.TryParse(clientIpAddress, out var ipAddress))
                return true;

            if (!ConnectionOccurrences.ContainsKey(clientIpAddress))
            {
                ConnectionOccurrences[clientIpAddress] = new List<DateTime>();
            }

            ConnectionOccurrences[clientIpAddress].Add(DateTime.Now);

            if (ConnectionOccurrences.TryGetValue(clientIpAddress, out List<DateTime> timestamps))
            {
                var now = DateTime.Now;
                var recentTimestamps = timestamps.FindAll(ts => (now - ts).TotalSeconds <= 30);
                return recentTimestamps.Count >= 2;
            }

            return false;
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            if (result.AsyncState is not GameClient client)
                return;

            try
            {
                var bytesRecv = client.EndReceive(result);

                if (bytesRecv >= 6 && bytesRecv <= short.MaxValue)
                {
                    OnDataReceived(new GameClientDataEvent(client, client.ReceiveBuffer));

                    if (client.IsConnected)
                        client.BeginReceive(ReceiveCallback, client);
                    else
                        RemoveClient(client, true);
                }
                else
                    RemoveClient(client, true);
            }
            catch (SocketException)
            {
                RemoveClient(client, true);
            }
            catch (Exception) { }
        }

        public void SendToAll(byte[] buffer)
        {
            foreach (var client in Clients)
            {
                if (client.IsConnected)
                    client.Send(buffer);
            }
        }

        public virtual int Send(GameClient client, byte[] buffer, int start, int count, SocketFlags flags)
        {
            CheckParametersValues(client, buffer);

            int totalBytesSent = 0;
            int bytesRemaining = buffer.Length;

            try
            {
                while (bytesRemaining > 0)
                {
                    if (client.IsConnected)
                    {
                        int bytesSent = client.Socket.Send(buffer, totalBytesSent, bytesRemaining, flags);

                        if (bytesSent > 0)
                            OnDataSent(new GameClientDataEvent(client, buffer));

                        bytesRemaining -= bytesSent;
                        totalBytesSent += bytesSent;
                    }
                    else
                        break;
                }
            }
            catch (SocketException)
            {
                RemoveClient(client, true);
            }

            return totalBytesSent;
        }

        private static void CheckParametersValues(GameClient client, byte[] buffer)
        {
            CheckClientParameter(client);

            CheckBufferParameter(buffer);
        }

        private static void CheckBufferParameter(byte[] buffer)
        {
            if (buffer == null || buffer.Length < 6)
            {
                ArgumentNullException nullException = new(nameof(buffer), NullObjectExceptionMessage);
                throw nullException;
            }
        }

        private static void CheckClientParameter(GameClient client)
        {
            if (client == null)
            {
                ArgumentNullException nullException = new(nameof(client), NullObjectExceptionMessage);
                throw nullException;
            }
        }

        public virtual void OnClientConnection(GameClientEvent e)
        {
            OnConnect?.Invoke(this, e);
        }

        public virtual void OnClientDisconnect(GameClientEvent e)
        {
            OnDisconnect?.Invoke(this, e);
        }

        public virtual void OnDataReceived(GameClientDataEvent e)
        {
            DataReceived?.Invoke(this, e, e.Data);
        }

        public virtual void OnDataSent(GameClientDataEvent e)
        {
            DataSent?.Invoke(this, e);
        }

        public virtual void DisconnectAll()
        {
            lock (ClientLock)
            {
                foreach (var client in Clients)
                {
                    if (client.IsConnected)
                    {
                        client.Socket.Disconnect(false);
                        OnClientDisconnect(new GameClientEvent(client));
                    }
                }

                Clients.Clear();
            }
        }

        public virtual void Disconnect(GameClient client, bool raiseEvent)
        {
            CheckClientParameter(client);

            if (!client.IsConnected)
                return;

            client.Socket.Disconnect(false);

            RemoveClient(client, raiseEvent);
        }

        public GameClient? FindByTamerId(long characterId)
        {
            return Clients.FirstOrDefault(x => x.TamerId == characterId);
        }

        public GameClient? FindByAccountId(long accountId)
        {
            return Clients.FirstOrDefault(x => x.AccountId == accountId);
        }

        public GameClient? FindByTamerName(string tamerName)
        {
            return Clients.FirstOrDefault(x => x.Tamer?.Name == tamerName);
        }

        public void RemoveClient(GameClient client, bool raiseEvent = false)
        {
            lock (ClientLock)
            {
                Clients.Remove(client);
            }

            if (raiseEvent)
                OnClientDisconnect(new GameClientEvent(client));
        }

        public virtual void Shutdown()
        {
            if (!IsListening)
                return;

            ServerListener?.Close();

            DisconnectAll();

            ServerListener?.Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
