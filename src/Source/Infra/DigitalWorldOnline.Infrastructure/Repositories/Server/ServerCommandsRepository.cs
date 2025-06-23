using AutoMapper;
using DigitalWorldOnline.Commons.Models.Mechanics;
using DigitalWorldOnline.Commons.Models.Servers;
using DigitalWorldOnline.Commons.Models.TamerShop;
using DigitalWorldOnline.Commons.DTOs.Mechanics;
using DigitalWorldOnline.Commons.DTOs.Server;
using DigitalWorldOnline.Commons.DTOs.Shop;
using Microsoft.EntityFrameworkCore;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Enums;
using DigitalWorldOnline.Commons.DTOs.Events;
using AutoMapper.Execution;
using DigitalWorldOnline.Commons.DTOs.Character;
using System;
using Microsoft.Data.SqlClient;

namespace DigitalWorldOnline.Infrastructure.Repositories.Server
{
    public class ServerCommandsRepository : IServerCommandsRepository
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public ServerCommandsRepository(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ConsignedShopDTO?> AddConsignedShopAsync(ConsignedShop personalShop)
        {
            var dto = _mapper.Map<ConsignedShopDTO>(personalShop);

            var latestItem = await _context.CharacterConsignedShop
                     .AsNoTracking()
                     .Include(x => x.Location)
                     .OrderByDescending(x => x.Id)
                     .FirstOrDefaultAsync();

            if(latestItem != null)
            {
                dto.SetGeneralHandler(latestItem.Id + 1);
            }
            else
            {
                _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('Shop.ConsignedShop', RESEED, 0)");
                _context.Database.ExecuteSqlRaw("DBCC CHECKIDENT('Shop.Location', RESEED, 0)");
                dto.SetGeneralHandler();
            }

            _context.CharacterConsignedShop.Add(dto);
            _context.SaveChanges();





            return dto;
        }

        public async Task<GuildDTO> AddGuildAsync(GuildModel guild)
        {
            var dto = _mapper.Map<GuildDTO>(guild);

            _context.Add(dto);

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task AddGuildHistoricEntryAsync(long guildId, GuildHistoricModel historicEntry)
        {
            var dto = await _context.Guild
                .AsNoTracking()
                .Include(x => x.Historic)
                .SingleOrDefaultAsync(x => x.Id == guildId);

            if (dto != null)
            {
                dto.Historic.Add(_mapper.Map<GuildHistoricDTO>(historicEntry));

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task AddGuildMemberAsync(long guildId, GuildMemberModel member)
        {
            var dto = await _context.Guild
                .AsNoTracking()
                .Include(x => x.Members)
                .SingleOrDefaultAsync(x => x.Id == guildId);

            if (dto != null)
            {
                dto.Members.Add(_mapper.Map<GuildMemberDTO>(member));

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task<ServerDTO?> AddServerAsync(ServerObject server)
        {
            var dto = _mapper.Map<ServerDTO>(server);

            _context.ServerConfig.Add(dto);

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task DeleteConsignedShopByHandlerAsync(long generalHandler)
        {
            var dto = await _context.CharacterConsignedShop
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GeneralHandler == generalHandler);

            if (dto != null)
            {
                _context.Remove(dto);

                _context.SaveChanges();
            }
        }

        public async Task DeleteGuildAsync(long guildId)
        {
            var dto = await _context.Guild.AsNoTracking().SingleOrDefaultAsync(x => x.Id == guildId);
            
            if (dto != null)
            {
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM [Guild].[Guild] WHERE Id = @guildId", new SqlParameter("@guildId", dto.Id));

                //_context.Remove(dto);
                _context.SaveChanges();
            }
        }

        public async Task DeleteGuildMemberAsync(long characterId, long guildId)
        {
            var guildDto = await _context.Guild
                .AsNoTracking()
                .Include(x => x.Members)
                .SingleOrDefaultAsync(x => x.Id == guildId);

            if (guildDto != null)
            {
                var memberDto = guildDto.Members.FirstOrDefault(x => x.CharacterId == characterId);

                if (memberDto != null)
                {
                    _context.Remove(memberDto);

                    guildDto.Members.RemoveAll(x => x.CharacterId == characterId);
                    _context.Update(guildDto);
                }

                _context.SaveChanges();
            }
        }

        public async Task<bool> DeleteServerAsync(long id)
        {
            var dto = await _context.ServerConfig
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (dto != null)
            {
                var chars = _context.Character.Count(x => x.ServerId == id);

                if (chars == default)
                {
                    _context.Remove(dto);
                    _context.SaveChanges();
                }
                else
                    return false;
            }

            return true;
        }

        public async Task UpdateArenaRankingAsync(ArenaRankingModel arena)
        {
            var dto = await _context.ArenaRanking
                .AsNoTracking()
                .Include(x => x.Competitors)
                .FirstOrDefaultAsync(x => x.Id == arena.Id);

            if (dto != null)
            {
                var memberToAdd = arena.Competitors
                    .Where(competitor => dto.Competitors.All(q => q.Id != competitor.Id))
                    .ToList();

                foreach (var newCompetitor in memberToAdd)
                {
                    var competitorDto = _mapper.Map<ArenaRankingCompetitorDTO>(newCompetitor);
                    competitorDto.RankingId = arena.Id;

                    _context.Competitor.Add(competitorDto);



                    _context.SaveChanges();
                }

                var dtoCompetitors = dto.Competitors.ToList();

                foreach (var existingCompetitor in dtoCompetitors)
                {
                    var updatedCompetitor = arena.Competitors.FirstOrDefault(q => q.Id == existingCompetitor.Id);

                    if (updatedCompetitor != null)
                    {
                        existingCompetitor.InsertDate = updatedCompetitor.InsertDate;
                        existingCompetitor.Position = updatedCompetitor.Position;
                        existingCompetitor.Points = updatedCompetitor.Points;
                        existingCompetitor.New = updatedCompetitor.New;

                        _context.Competitor.Update(existingCompetitor);
                    }
                }

                _context.SaveChanges();
            }
        }


        public async Task UpdateGuildAuthorityAsync(GuildAuthorityModel authority)
        {
            var dto = await _context.GuildAuthority
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == authority.Id);

            if (dto != null)
            {
                dto.Duty = authority.Duty;
                dto.Title = authority.Title;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task UpdateGuildMemberAuthorityAsync(GuildMemberModel guildMember)
        {
            var dto = await _context.GuildMember
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == guildMember.Id);

            if (dto != null)
            {
                dto.Authority = guildMember.Authority;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task UpdateGuildNoticeAsync(long guildId, string newMessage)
        {
            var dto = await _context.Guild
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == guildId);

            if (dto != null)
            {
                dto.Notice = newMessage;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }

        public async Task UpdateServerAsync(long serverId, string serverName, int experience, bool maintenance)
        {
            var dto = await _context.ServerConfig
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == serverId);

            if (dto != null)
            {
                dto.Name = serverName;
                dto.Experience = experience;
                dto.Maintenance = maintenance;

                _context.Update(dto);
                _context.SaveChanges();
            }
        }
    }
}