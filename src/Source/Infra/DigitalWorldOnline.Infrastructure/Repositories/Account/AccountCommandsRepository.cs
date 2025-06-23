using AutoMapper;
using DigitalWorldOnline.Commons.DTOs.Account;
using DigitalWorldOnline.Commons.DTOs.Character;
using DigitalWorldOnline.Commons.DTOs.Config;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.Extensions;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Models.Account;
using DigitalWorldOnline.Commons.Models.Config;
using DigitalWorldOnline.Commons.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace DigitalWorldOnline.Infrastructure.Repositories.Account
{
    public class AccountCommandsRepository : IAccountCommandsRepository
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public AccountCommandsRepository(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AccountDTO> AddAccountAsync(AccountModel account)
        {
            var dto = _mapper.Map<AccountDTO>(account);

            _context.Add(dto);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<AccountBlockDTO> AddAccountBlockAsync(AccountBlockModel account)
        {
            var dto = _mapper.Map<AccountBlockDTO>(account);

            _context.Add(dto);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<LoginTryDTO> AddLoginTryAsync(LoginTryModel loginTry)
        {
            var dto = _mapper.Map<LoginTryDTO>(loginTry);

            _context.Add(dto);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<SystemInformationDTO> AddSystemInformationAsync(SystemInformationModel systemInformation)
        {
            var dto = _mapper.Map<SystemInformationDTO>(systemInformation);

            _context.SystemInformation.Add(dto);

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task UpdateSystemInformationAsync(SystemInformationModel systemInformation)
        {
            var dto = _mapper.Map<SystemInformationDTO>(systemInformation);

            _context.SystemInformation.Update(dto);

            await _context.SaveChangesAsync();
        }

        public async Task CreateOrUpdateSecondaryPasswordByIdAsync(long accountId, string secondaryPassword)
        {
            var dto = await _context.Account
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == accountId);

            if (dto != null)
            {
                dto.SecondaryPassword = secondaryPassword;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task UpdateLastPlayedServerByIdAsync(long accountId, long serverId)
        {
            var dto = await _context.Account
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == accountId);

            if (dto != null)
            {
                dto.LastPlayedServer = serverId;
                dto.LastConnection = DateTime.Now;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task UpdateLastPlayedCharacterByIdAsync(long accountId, long characterId)
        {
            var dto = await _context.Account
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == accountId);

            if (dto != null)
            {
                dto.LastPlayedCharacter = characterId;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task UpdatePremiumAndSilkByIdAsync(long accountId, int premium, int silk)
        {
            var dto = await _context.Account
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == accountId);

            if (dto != null)
            {
                dto.Premium = premium;
                dto.Silk = silk;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task UpdateAccountAsync(AccountModel account)
        {
            var dto = await _context.Account
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == account.Id);

            if (dto != null)
            {
                dto.Username = account.Username;
                dto.Password = account.Password.Encrypt();
                dto.Email = account.Email;
                dto.AccessLevel = account.AccessLevel;
                dto.Premium = account.Premium;
                dto.Silk = account.Silk;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }
        
        public async Task UpdateAccountMembershipAsync(long accountId, DateTime? expirationDate)
        {
            var dto = await _context.Account
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == accountId);

            if (dto != null)
            {
                dto.MembershipExpirationDate = expirationDate;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task DeleteAccountAsync(long id)
        {
            var dto = await _context.Account
                .Include(x => x.SystemInformation)
                .Include(x => x.AccountBlock)
                .Include(x => x.ItemList)
                    .ThenInclude(y => y.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dto != null)
            {
                _context.Remove(dto);
                _context.SaveChanges();
            }
        }

        public async Task UpdateAccountWelcomeFlagAsync(long accountId, bool welcomeFlag)
        {
            var dto = await _context.Account.FirstOrDefaultAsync(x => x.Id == accountId);

            if (dto != null)
            {
                dto.ReceiveWelcome = welcomeFlag;

                _context.Update(dto);

                _context.SaveChanges();
            }
        }

        public async Task RemoveActiveQuestAsync(Guid? progressQuestId)
        {
            var dto = await _context.InProgressQuest
                .FirstOrDefaultAsync(x => x.Id == progressQuestId);

            if(dto != null)
            {
                _context.Remove(dto);
                _context.SaveChanges();
            }
        }
      
    }
}
