namespace UEFA.ChampionsLeague.Contracts.DataTransferObjects.Matches
{
    public class MatchDto
    {
        public string LeagueTitle { get; set; }
        public int MatchDay { get; set; }
        public string Group { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string Score { get; set; }
        public string KickoffAt { get; set; }
    }
}
