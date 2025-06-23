using DigitalWorldOnline.Commons.Enums.ClientEnums;
using DigitalWorldOnline.Commons.Enums.Server;
using System.ComponentModel.DataAnnotations;

namespace DigitalWorldOnline.Commons.DTOs.Server
{
    public class ServerDTO
    {
        public long Id { get; set; }

        [MinLength(6)]
        public string Name { get; set; }

        public int Experience { get; set; }

        public bool Maintenance { get; set; }

        public bool New { get; set; } = true;

        public ServerOverloadEnum Overload { get; set; }
        
        public ServerTypeEnum Type { get; set; }
        
        public int Port { get; set; }

        public DateTime CreateDate { get; set; }
    }
}