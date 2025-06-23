using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.Server;

namespace DigitalWorldOnline.Commons.Models.Servers
{
    public class ServerObject //TODO: renomear namespace
    {
        public long Id { get; private set; }
        public string Name { get; set; }
        public int Experience { get; set; }
        public bool Maintenance { get; set; }
        public bool New { get; private set; }
        public int Port { get; private set; }
        public ServerOverloadEnum Overload { get; private set; }
        public ServerTypeEnum Type { get; private set; }
        public DateTime CreateDate { get; private set; }

        public byte CharacterCount { get; private set; }

        public void UpdateCharacterCount(byte count)
        {
            CharacterCount = count;
        }
    }
}
