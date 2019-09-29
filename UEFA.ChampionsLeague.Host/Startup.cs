using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using System;
using UEFA.ChampionsLeague.Business.Extensions;
using UEFA.ChampionsLeague.Host.Middleware;

namespace UEFA.ChampionsLeague.Host
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Override default model validation middleware.
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddMvcCore(options =>
            {
                options.Filters.Add(typeof(ModelValidationMiddleware));
            })
            .AddDataAnnotations()
            .AddJsonFormatters()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerDocument(options =>
            {
                options.DocumentName = "v1";
                options.Version = "1";

                options.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "UEFA Champions League API";
                    document.Info.Description = "Proof of concept";
                    document.Info.TermsOfService = "";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Email = "miodragradojkovic1@gmail.com",
                        Name = "Miodrag Radojkovic"
                    };
                };
            });

            services.RegisterBusinessServices(_configuration, _environment);
        }

        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<AutofacModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IComponentContext componentContext,
            IServiceProvider service
            )
        {
            if (componentContext == null) throw new ArgumentNullException(nameof(componentContext));
            if (service == null) throw new ArgumentNullException(nameof(service));

            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
            app.UseMvc();

            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.SwaggerRoutes.Add(new SwaggerUi3Route("v1", "/swagger/v1/swagger.json"));
            });

            app.ConfigureBusinessServices(_environment, componentContext);
        }
    }
}
