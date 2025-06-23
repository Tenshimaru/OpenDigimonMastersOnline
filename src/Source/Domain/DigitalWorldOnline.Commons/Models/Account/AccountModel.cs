using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Enums.Account;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Models.Base;

namespace DigitalWorldOnline.Commons.Models.Account
{
    public class AccountModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
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
        public SystemInformationModel? SystemInformation { get; set; }
        public AccountBlockModel? AccountBlock { get; set; }
        public List<ItemListModel> ItemList { get; private set; }
        public bool ReceiveWelcome { get; private set; }

        public static AccountModel Create(string username, string password,
            string email, string? secondaryPassword = null,
            AccountAccessLevelEnum accessLevel = AccountAccessLevelEnum.Default,
            int premium = 0, int silk = 0,
            SystemInformationModel? systemInformation = null,
            AccountBlockModel? accountBlock = null)
        {
            return new AccountModel()
            {
                Username = username,
                Password = password,
                Email = email,
                SecondaryPassword = secondaryPassword,
                AccessLevel = accessLevel,
                CreateDate = DateTime.Now,
                Premium = premium,
                Silk = silk,
                LastPlayedServer = 0,
                SystemInformation = systemInformation,
                AccountBlock = accountBlock,
                DiscordId = "0",
                MembershipExpirationDate = DateTime.Now,
                
                ItemList = new List<ItemListModel>()
                {
                    new ItemListModel(ItemListEnum.AccountWarehouse),
                    new ItemListModel(ItemListEnum.CashWarehouse),
                    new ItemListModel(ItemListEnum.ShopWarehouse),
                    new ItemListModel(ItemListEnum.BuyHistory)
                }
            };
        }
        
        public static AccountModel Create(
            string username,
            string email,
            string discordId,
            string password)
        {
            return new AccountModel()
            {
                Username = username,
                Password = password.Encrypt(),
                Email = email,
                AccessLevel = AccountAccessLevelEnum.Default,
                CreateDate = DateTime.Now,
                DiscordId = discordId,
                ItemList = new List<ItemListModel>()
                {
                    new ItemListModel(ItemListEnum.AccountWarehouse),
                    new ItemListModel(ItemListEnum.CashWarehouse),
                    new ItemListModel(ItemListEnum.ShopWarehouse),
                    new ItemListModel(ItemListEnum.BuyHistory)
                }
            };
        }

        public bool CharacterDeleteValidation(string validation)
        {
            return validation == Email || validation == SecondaryPassword;
        }

        public void BlockUnblock()
        {
            if (AccessLevel == AccountAccessLevelEnum.Blocked)
                AccessLevel = AccountAccessLevelEnum.Default;
            else
                AccessLevel = AccountAccessLevelEnum.Blocked;
        }
    }
}