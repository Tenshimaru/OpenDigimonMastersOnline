namespace DigitalWorldOnline.Account.Models.Configuration;

public class AuthenticationServerConfigurationModel
{
    public string Address { get; set; }
    public int Port { get; set; }
    public string Backlog { get; set; }
    public bool UseHash { get; set; }
    public bool AllowRegisterOnLogin { get; set; }
}