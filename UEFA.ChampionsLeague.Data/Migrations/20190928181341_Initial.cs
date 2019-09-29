using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UEFA.ChampionsLeague.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LeagueTitle = table.Column<string>(type: "nvarchar(250)", nullable: true),
                    MatchDay = table.Column<int>(nullable: false),
                    Group = table.Column<string>(type: "nvarchar(1)", nullable: true),
                    HomeTeam = table.Column<string>(type: "nvarchar(250)", nullable: true),
                    AwayTeam = table.Column<string>(type: "nvarchar(250)", nullable: true),
                    HomeTeamScore = table.Column<byte>(nullable: false),
                    AwayTeamScore = table.Column<byte>(nullable: false),
                    KickoffAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");
        }
    }
}
