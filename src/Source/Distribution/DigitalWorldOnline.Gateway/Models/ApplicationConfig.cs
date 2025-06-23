namespace DigitalWorldOnline.Gateway.Models
{
    public class ApplicationConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}