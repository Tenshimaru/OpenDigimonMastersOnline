using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.Models.Character;
using DigitalWorldOnline.Commons.Models.Digimon;
using DigitalWorldOnline.Commons.Utils;
using DigitalWorldOnline.Commons.Writers;
using System.Net.Sockets;

namespace DigitalWorldOnline.Commons.Entities
{

    public class GameClient
    {
        //TODO: Behavior
        //TODO: separar funções do comutador e do banco
        public GameServer Server { get; private set; }

        public Socket Socket { get; private set; }

        public byte[] ReceiveBuffer { get; private set; }

        public bool IsConnected => Socket.Connected;

        public string ClientAddress => Socket?.RemoteEndPoint?.ToString();

        public string HiddenAddress
        {
            get
            {
                if(string.IsNullOrEmpty(ClientAddress))
                    return string.Empty;

                if (ClientAddress.Length <= 6)
                {
                    return new string('*', ClientAddress.Length);
                }
                else
                {
                    var shown = ClientAddress.Substring(0, 6);
                    var hidden = new string('*', ClientAddress.Length - 6);
                    return shown + hidden;
                }
            }
        }

        public short Handshake { get; private set; }

        public long AccountId { get; private set; }
        public string AccountEmail { get; private set; }
        public string? AccountSecondaryPassword { get; private set; }
        public long ServerId { get; private set; }

        //Temp
        public int ServerExperience { get; private set; }

        public DateTime? MembershipExpirationDate { get; private set; }
        public int Premium { get; set; }
        public int Silk { get; private set; }
        public AccountAccessLevelEnum AccessLevel { get; private set; }
        public bool Loading { get; private set; }

        public CharacterModel Tamer { get; private set; }

        public DigimonModel Partner => Tamer.Partner;

        public long TamerId => Tamer?.Id ?? 0;

        public string TamerLocation => $"Map {Tamer?.Location.MapId} X{Tamer?.Location.X} Y{Tamer?.Location.Y}, Channel {Tamer?.Channel}";

        public bool ReceiveWelcome { get; private set; }

        public bool GameQuit { get; private set; }

        // -------------------------------------------------------------------------------------------------------

        public bool DungeonMap => UtilitiesFunctions.DungeonMapIds.Contains(Tamer?.Location.MapId ?? 0);

        public bool EventMap => UtilitiesFunctions.EventMapIds.Contains(Tamer?.Location.MapId ?? 0);

        public bool PvpMap => UtilitiesFunctions.PvpMapIds.Contains(Tamer?.Location.MapId ?? 0);

        // -------------------------------------------------------------------------------------------------------

        public bool SentOnceDataSent { get; private set; }

        private const int BufferSize = 16 * 1024;

        public GameClient(GameServer server, Socket socket)
        {
            ReceiveBuffer = new byte[BufferSize];

            Server = server;
            Socket = socket;
            GameQuit = true;
            SentOnceDataSent = false;
        }

        public int MembershipUtcSeconds => MembershipExpirationDate.GetUtcSeconds();            //  Membership 
        public int MembershipUtcSecondsBuff => MembershipExpirationDate.GetUtcSecondsBuff();    //  Membership for buffs
        
        public int PartnerDeleteValidation(string validation)
        {
            if (!string.IsNullOrEmpty(AccountSecondaryPassword))
            {
                if (validation == AccountSecondaryPassword)
                    return 1;
                else
                    return -1;
            }
            else
            {
                if (validation == AccountEmail)
                    return 1;
                else
                    return -2;
            }
        }

        public void SetAccountId(long accountId)
        {
            AccountId = accountId;
        }

        public void SetLoading(bool loading = true)
        {
            Loading = loading;
        }

        public void AddPremium(int premium)
        {
            if (Premium + premium > int.MaxValue)
                Premium = int.MaxValue;
            else
                Premium += premium;
        }

        public void AddSilk(int silk)
        {
            if (Silk + silk > int.MaxValue)
                Silk = int.MaxValue;
            else
                Silk += silk;
        }

        public void SetServerExperience(int experience) => ServerExperience = experience;

        public void SetAccessLevel(AccountAccessLevelEnum accessLevel)
        {
            AccessLevel = accessLevel;
        }

        public void SetAccountInfo(AccountModel account)
        {
            if (account == null)
                return;

            AccountId = account.Id;
            AccountEmail = account.Email;
            AccountSecondaryPassword = account.SecondaryPassword;
            ServerId = account.LastPlayedServer;
            MembershipExpirationDate = account.MembershipExpirationDate;
            Premium = account.Premium;
            Silk = account.Silk;
            AccessLevel = account.AccessLevel;
            ReceiveWelcome = account.ReceiveWelcome;
        }

        // ----------------------------------------------------------------------------------

        public void IncreaseMembershipDuration(int seconds)
        {
            if (MembershipExpirationDate == null)
            {
                MembershipExpirationDate = DateTime.UtcNow.AddSeconds(seconds);
            }
            else
            {
                if (MembershipExpirationDate < DateTime.UtcNow)
                {
                    MembershipExpirationDate = DateTime.UtcNow.AddSeconds(seconds);
                }
                else
                    MembershipExpirationDate = MembershipExpirationDate.Value.AddSeconds(seconds);
            }
        }

        public void RemoveMembership()
        {
            MembershipExpirationDate = DateTime.UtcNow;
        }

        // ----------------------------------------------------------------------------------

        public void SetCharacter(CharacterModel character)
        {
            Tamer = character;
        }

        public void Disable()
        {
            Socket.EnableBroadcast = false;
        }

        /// <summary>
        /// Flag for game quit (quit, kick, DC, game crash, etc).
        /// </summary>
        /// <param name="gameQuit">Quit value</param>
        public void SetGameQuit(bool gameQuit)
        {
            GameQuit = gameQuit;
        }

        /// <summary>
        /// Flag for game to send data that should be only sent once.
        /// </summary>
        /// <param name="sentOnceDataSent">Set sent once data</param>
        public void SetSentOnceDataSent(bool sentOnceDataSent)
        {
            SentOnceDataSent = sentOnceDataSent;
        }

        public void Enable()
        {
            Socket.EnableBroadcast = true;
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return Socket.BeginReceive(ReceiveBuffer, 0, BufferSize, SocketFlags.None, callback, state);
        }

        public int EndReceive(IAsyncResult result)
        {
            return Socket.EndReceive(result);
        }

        public void SendToAll(byte[] buffer)
        {
            foreach (var client in Server.Clients)
            {
                if (client.IsConnected)
                    client.Send(buffer);
            }
        }

        public void SetHandshake(short handshake)
        {
            Handshake = handshake;
        }

        public void Send(PacketWriter packet) => Send(packet.Serialize());

        public int Send(byte[] buffer)
        {
            if (!IsConnected || buffer.Length < 6)
                return 0;

            return Send(buffer, 0, buffer.Length);
        }

        private int Send(byte[] buffer, int start, int count)
        {
            return Server.Send(this, buffer, start, count, SocketFlags.None);
        }

        public void Disconnect(bool raiseEvent = false)
        {
            if (IsConnected)
                Server.Disconnect(this, raiseEvent);
        }
    }
}
