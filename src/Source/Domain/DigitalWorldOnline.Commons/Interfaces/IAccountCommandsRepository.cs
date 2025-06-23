using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.Models.Security;
using DigitalWorldOnline.Commons.DTOs.Account;

namespace DigitalWorldOnline.Commons.Interfaces
{
    public interface IAccountCommandsRepository
    {
        Task<AccountDTO> AddAccountAsync(AccountModel account);
        
        Task<AccountBlockDTO> AddAccountBlockAsync(AccountBlockModel account);

        Task<LoginTryDTO> AddLoginTryAsync(LoginTryModel loginTry);

        Task<SystemInformationDTO> AddSystemInformationAsync(SystemInformationModel systemInformation);

        Task UpdateSystemInformationAsync(SystemInformationModel systemInformation);

        Task CreateOrUpdateSecondaryPasswordByIdAsync(long accountId, string secondaryPassword);

        Task UpdateAccountWelcomeFlagAsync(long accountId, bool welcomeFlag);

        Task DeleteAccountAsync(long id);

        Task UpdateAccountAsync(AccountModel account);

        Task UpdateAccountMembershipAsync(long accountId, DateTime? expirationDate);

        Task UpdatePremiumAndSilkByIdAsync(long accountId, int premium, int silk);

        Task UpdateLastPlayedServerByIdAsync(long accountId, long serverId);
        
        Task UpdateLastPlayedCharacterByIdAsync(long accountId, long characterId);

        Task RemoveActiveQuestAsync(Guid? progressQuestId);
    }
}
