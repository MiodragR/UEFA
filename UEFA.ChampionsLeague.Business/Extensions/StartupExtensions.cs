using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UEFA.ChampionsLeague.Data.Extensions;

namespace UEFA.ChampionsLeague.Business.Extensions
{
    public static class StartupExtensions
    {
        public static void RegisterBusinessServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IHostingEnvironment environment
        )
        {
            services.RegisterDataServices(configuration, environment);
        }

        public static IApplicationBuilder ConfigureBusinessServices(
            this IApplicationBuilder app,
            IHostingEnvironment env,
            IComponentContext componentContext
        )
        {
            app.ConfigureDataServices(componentContext);

            return app;
        }
    }
}
