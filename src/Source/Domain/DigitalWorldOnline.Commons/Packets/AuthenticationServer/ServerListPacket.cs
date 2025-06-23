using DigitalWorldOnline.Commons.Models.Servers;
using DigitalWorldOnline.Commons.Writers;

namespace DigitalWorldOnline.Commons.Packets.AuthenticationServer
{
    public class ServerListPacket : PacketWriter
    {
        private const int PacketNumber = 1701;

        /// <summary>
        /// The servers list with account's character count.
        /// </summary>
        public ServerListPacket(IEnumerable<ServerObject> servers)
        {
            var serverObjects = servers.ToList();

            Type(PacketNumber);
            WriteByte((byte)serverObjects.Count());

            foreach (var server in serverObjects)
            {
                WriteInt((int)server.Id);
                WriteString(server.Name);
                WriteByte(Convert.ToByte(server.Maintenance));
                WriteByte(Convert.ToByte(server.Overload));
                WriteByte(server.CharacterCount);
                WriteByte(Convert.ToByte(server.New));
            }

            WriteInt(0);
        }
    }
}