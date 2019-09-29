using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UEFA.ChampionsLeague.Data.Extensions
{
    public static class StartupExtensions
    {
        public static void RegisterDataServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment
        )
        {
            var connectionString = configuration.GetConnectionString(nameof(UEFAChampionsLeagueDbContext));
            services.AddDbContext<UEFAChampionsLeagueDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString);
                    options.EnableSensitiveDataLogging(environment?.IsDevelopment() ?? true);
                }
            );
        }

        public static IApplicationBuilder ConfigureDataServices(this IApplicationBuilder app, IComponentContext componentContext)
        {
            componentContext.Resolve<UEFAChampionsLeagueDbContext>().Database.Migrate();

            return app;
        }
    }
}
