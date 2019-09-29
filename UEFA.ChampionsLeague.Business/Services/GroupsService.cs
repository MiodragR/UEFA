using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UEFA.ChampionsLeague.Contracts;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects.Groups;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects.Matches;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects.Standings;
using UEFA.ChampionsLeague.Contracts.Enums;
using UEFA.ChampionsLeague.Data.Models;
using UEFA.ChampionsLeague.Data.Services;

namespace UEFA.ChampionsLeague.Business.Services
{
    public class GroupsService
    {
        private readonly GroupsDataService _service;
        public GroupsService(GroupsDataService service)
        {
            _service = service;
        }

        public async Task<ResponseTemplateViewDto<bool>> CreateAsync(List<MatchDto> dtos)
        {
            var existingMatches = await _service.GetAByTeamsAsync(dtos);

            var newMatchesOnly = dtos.Where(x =>
                existingMatches.All(y => y.HomeTeam != x.HomeTeam) || existingMatches.All(y => y.AwayTeam != x.AwayTeam)).ToList();

            if (newMatchesOnly.Any())
            {
                var matches = new List<Match>();

                newMatchesOnly.ForEach(newMatch =>
                {
                    var homeTeamScore = byte.Parse(newMatch.Score.Split(':')[0]);
                    var awayTeamScore = byte.Parse(newMatch.Score.Split(':')[1]);

                    matches.Add(new Match
                    {
                        LeagueTitle = newMatch.LeagueTitle,
                        Group = newMatch.Group,
                        MatchDay = newMatch.MatchDay,
                        HomeTeam = newMatch.HomeTeam,
                        AwayTeam = newMatch.AwayTeam,
                        HomeTeamScore = homeTeamScore,
                        AwayTeamScore = awayTeamScore,
                        KickoffAt = DateTime.ParseExact(newMatch.KickoffAt.Replace("-", "/"), "yyyy/MM/ddTHH:mm:ssK",
                            System.Globalization.CultureInfo.InvariantCulture)
                    });
                });

                await _service.CreateAsync(matches);

                return new ResponseTemplateViewDto<bool> { IsSuccess = true, Data = true };
            }

            throw new UEFAException("Send new matches only please", UEFAExceptionType.Business);
        }

        public async Task<ResponseTemplateViewDto<List<GroupsNavigationDto>>> GetGroupedByGroupNameAsync(List<string> queryGroup)
        {
            var groupedMatches = await _service.GetGroupedByGroupNameAsync(queryGroup);
            var groups = new List<GroupsNavigationDto>();

            foreach (var matchesInOneGroup in groupedMatches)
            {
                groups.Add(new GroupsNavigationDto()
                {
                    Group = matchesInOneGroup.Select(x => x.Group).FirstOrDefault(),
                    MatchDay = matchesInOneGroup.Select(x => x.MatchDay).FirstOrDefault(),
                    LeagueTitle = matchesInOneGroup.Select(x => x.LeagueTitle).FirstOrDefault(),
                    Standings = CalculateStandings(matchesInOneGroup)
                });
            }

            return new ResponseTemplateViewDto<List<GroupsNavigationDto>>(){ Data = groups, IsSuccess = true };
        }

        public async Task<ResponseTemplateViewDto<List<MatchDto>>> SearchAsync(string fromDate, string toDate, string queryTeam, string queryGroup)
        {
            var isFromDate = DateTime.TryParse(fromDate, out var fDate);
            var isToDate = DateTime.TryParse(toDate, out var tDate);

            var groupsFilter = new List<string>();
            if (!string.IsNullOrEmpty(queryGroup))
            {
                groupsFilter.Add(queryGroup);
            }

            var allMatches = await _service.GetFilteredAsync(groupsFilter, isFromDate ? fDate : (DateTime?)null, isToDate ? tDate : (DateTime?)null, queryTeam);
            var matches = allMatches.Select(x => new MatchDto()
            {
                Group = x.Group,
                AwayTeam = x.AwayTeam,
                HomeTeam = x.HomeTeam,
                MatchDay = x.MatchDay,
                LeagueTitle = x.LeagueTitle,
                KickoffAt = x.KickoffAt.ToString("U"),
                Score = $"{x.HomeTeamScore} : {x.AwayTeamScore}"
            }).ToList();

            return new ResponseTemplateViewDto<List<MatchDto>>() { Data = matches, IsSuccess = true };
        }

        public async Task<ResponseTemplateViewDto<bool>> UpdateAsync(List<MatchDto> dtos)
        {
            var existingMatches = await _service.GetAByTeamsAsync(dtos);

            foreach (var existingMatch in existingMatches)
            {
                var score = dtos.Where(x => x.HomeTeam == existingMatch.HomeTeam && x.AwayTeam == existingMatch.AwayTeam).Select(x => x.Score).FirstOrDefault();
                if (string.IsNullOrEmpty(score))
                {
                    throw new UEFAException($"Please send valid score value in your request for the match between: {existingMatch.HomeTeam} and {existingMatch.AwayTeam} on {existingMatch.KickoffAt}", UEFAExceptionType.Business);
                }
                var homeTeamScore = byte.Parse(score.Split(':')[0]);
                var awayTeamScore = byte.Parse(score.Split(':')[1]);
                existingMatch.HomeTeamScore = homeTeamScore;
                existingMatch.AwayTeamScore = awayTeamScore;
            }

            await _service.UpdateAsync(existingMatches);

            return new ResponseTemplateViewDto<bool> { IsSuccess = true, Data = true };
        }

        #region Private
        private static List<StandingNavigationDto> CalculateStandings(IReadOnlyCollection<Match> matches)
        {
            var index = 1;
            return (
                    from match in (
                        matches.Select(x => new
                            {
                                team = x.HomeTeam,
                                x.HomeTeamScore,
                                x.AwayTeamScore
                            })
                            .Concat(matches.Select(y => new
                            {
                                team = y.AwayTeam,
                                HomeTeamScore = y.AwayTeamScore,
                                AwayTeamScore = y.HomeTeamScore
                            }))
                        )
                    group match by new
                    {
                        match.team
                    }
                    into g
                    orderby
                        g.Sum(p => (p.HomeTeamScore > p.AwayTeamScore ? 3 : 0)) descending,
                        g.Sum(p => p.HomeTeamScore) - g.Sum(p => p.AwayTeamScore) descending
                    select new StandingNavigationDto()
                    {
                        Rank = index++,
                        Team = g.Key.team,
                        PlayedGames = g.Count(),
                        Win = g.Count(p => (p.HomeTeamScore > p.AwayTeamScore ? (long?)1 : null) != null),
                        Lose = g.Count(p => (p.AwayTeamScore > p.HomeTeamScore ? (long?)1 : null) != null),
                        Draw = g.Count(p => (p.HomeTeamScore == p.AwayTeamScore ? (long?)1 : null) != null),
                        Goals = g.Sum(p => p.HomeTeamScore),
                        GoalsAgainst = g.Sum(p => p.AwayTeamScore),
                        GoalDifference = (g.Sum(p => p.HomeTeamScore) - g.Sum(p => p.AwayTeamScore)),
                        Points = g.Sum(p => (p.HomeTeamScore > p.AwayTeamScore ? 3 : 0))
                    }).ToList();
        }
        #endregion
    }
}
