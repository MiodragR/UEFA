using System.Collections.Generic;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects.Standings;

namespace UEFA.ChampionsLeague.Contracts.DataTransferObjects.Groups
{
    public class GroupsNavigationDto
    {
        public string LeagueTitle { get; set; }
        public int MatchDay { get; set; }
        public string Group { get; set; }

        public List<StandingNavigationDto> Standings { get; set; }
    }
}
