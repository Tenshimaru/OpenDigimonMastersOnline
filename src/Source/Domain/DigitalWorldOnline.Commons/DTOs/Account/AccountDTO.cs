using DigitalWorldOnline.Commons.DTOs.Base;
using DigitalWorldOnline.Commons.Enums.Account;
using System.ComponentModel.DataAnnotations;

namespace DigitalWorldOnline.Commons.DTOs.Account
{
    public class AccountDTO
    {
        public long Id { get; set; }

        [MinLength(6)]
        public string Username { get; set; }
        
        [MinLength(6)]
        public string Password { get; set; }

        public string? SecondaryPassword { get; set; }

        public string Email { get; set; }
        
        public string DiscordId { get; set; }

        public AccountAccessLevelEnum AccessLevel { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastConnection { get; set; }

        public DateTime? MembershipExpirationDate { get; set; }

        public int Premium { get; set; }

        public int Silk { get; set; }

        public long LastPlayedServer { get; set; }
        
        public long LastPlayedCharacter { get; set; }

        public bool ReceiveWelcome { get; set; } = true;

        //FK
        public SystemInformationDTO? SystemInformation { get; set; }
        public AccountBlockDTO? AccountBlock { get; set; }
        public List<ItemListDTO> ItemList { get; set; }
    }
}
