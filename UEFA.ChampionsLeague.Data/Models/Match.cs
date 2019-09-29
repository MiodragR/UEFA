using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UEFA.ChampionsLeague.Data.Models
{
    public class Match
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string LeagueTitle { get; set; }
        public int MatchDay { get; set; }
        [Column(TypeName = "nvarchar(1)")]
        public string Group { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string HomeTeam { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string AwayTeam { get; set; }
        public byte HomeTeamScore { get; set; }
        public byte AwayTeamScore { get; set; }
        public DateTime KickoffAt { get; set; }
    }
}
