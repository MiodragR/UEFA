using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UEFA.ChampionsLeague.Contracts;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects.Matches;
using UEFA.ChampionsLeague.Contracts.Enums;
using UEFA.ChampionsLeague.Data.Models;

namespace UEFA.ChampionsLeague.Data.Services
{
    public class GroupsDataService
    {
        private readonly UEFAChampionsLeagueDbContext _context;
        public GroupsDataService(UEFAChampionsLeagueDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(List<Match> matches)
        {
            try
            {
                await _context.Matches.AddRangeAsync(matches);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new UEFAException("Create unsuccessful, check your input and try again.", UEFAExceptionType.Data);
            }
        }

        public async Task<List<List<Match>>> GetGroupedByGroupNameAsync(List<string> queryGroup)
        {
            var result = await _context.Matches.Where(x => queryGroup.Contains(x.Group) || queryGroup.All(i => i == null))
                .GroupBy(x => x.Group)
                .Select(grp => grp.OrderByDescending(x => x.MatchDay).ToList())
                .ToListAsync();

            if (!result.Any())
            {
                throw new UEFAException("No search results.", UEFAExceptionType.Data);
            }

            return result;
        }

        public async Task<List<Match>> GetFilteredAsync(
            List<string> queryString,
            DateTime? fromDate,
            DateTime? toDate,
            string queryTeam
        )
        {
            var result = await _context.Matches
                .Where(x =>
                    (!queryString.Any() || queryString.Contains(x.Group)) &&
                    (string.IsNullOrEmpty(queryTeam) || x.HomeTeam.ToLower().Contains(queryTeam.ToLower()) ||
                     string.IsNullOrEmpty(queryTeam) || x.AwayTeam.ToLower().Contains(queryTeam.ToLower())) &&
                    ((!fromDate.HasValue || fromDate.Value <= x.KickoffAt) &&
                     (!toDate.HasValue || toDate.Value >= x.KickoffAt))
                ).ToListAsync();

            return result;
        }

        public async Task<List<Match>> GetAByTeamsAsync(List<MatchDto> dtos)
        {
            return await (
                from match in _context.Matches
                join dto in dtos
                    on new { match.HomeTeam, match.AwayTeam } 
                    equals new { dto.HomeTeam, dto.AwayTeam }
                    into matches
                where matches.Any()
                select match).ToListAsync();
        }

        public async Task<bool> UpdateAsync(List<Match> matches)
        {
            try
            {
                _context.Matches.UpdateRange(matches);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new UEFAException("Update unsuccessful, check your input and try again.", UEFAExceptionType.Data);
            }
        }
    }
}
