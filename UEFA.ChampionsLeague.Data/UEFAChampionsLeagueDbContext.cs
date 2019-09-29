using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace UEFA.ChampionsLeague.Data
{
    public class UEFAChampionsLeagueDbContext : DbContext
    {
        public DbSet<Models.Match> Matches { get; set; }

        public UEFAChampionsLeagueDbContext(
            DbContextOptions<UEFAChampionsLeagueDbContext> options,
            IConfiguration configuration
        ) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Models.Match>().Property(x => x.Id).IsRequired();
        }
    }
}
