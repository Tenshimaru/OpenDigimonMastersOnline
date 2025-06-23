using System.Collections.Generic;

namespace DigitalWorldOnline.Gateway.Models
{
    public class GatewayConfig
    {
        public string GatewayIP { get; set; } = string.Empty;
        public int GatewayPort { get; set; }
        public List<ApplicationConfig> Applications { get; set; } = new();
    }
}