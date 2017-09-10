using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Bibliotheca.Server.Authorization.Heimdall.Core.Parameters;
using Bibliotheca.Server.Authorization.Heimdall.Core.Services;
using Hangfire;
using Bibliotheca.Server.Authorization.Heimdall.Api.Jobs;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;
using Bibliotheca.Server.Mvc.Middleware.Authorization.SecureTokenAuthentication;
using Bibliotheca.Server.Mvc.Middleware.Authorization.BearerAuthentication;
using Bibliotheca.Server.Mvc.Middleware.Authorization.UserTokenAuthentication;
using Bibliotheca.Server.Authorization.Heimdall.Api.UserTokenAuthorization;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using Neutrino.AspNetCore.Client;
using System.Linq;

namespace Bibliotheca.Server.Authorization.Heimdall.Api
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        private IConfigurationRoot Configuration { get; }

        private bool UseServiceDiscovery { get; set; } = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="env">Environment parameters.</param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        /// <summary>
        /// Service configuration.
        /// </summary>
        /// <param name="services">List of services.</param>
        /// <returns>Service provider.</returns>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationParameters>(Configuration);

            if (UseServiceDiscovery)
            {
                services.AddHangfire(x => x.UseStorage(new Hangfire.MemoryStorage.MemoryStorage()));
            }

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(SecureTokenSchema.Name)
                    .AddAuthenticationSchemes(UserTokenSchema.Name)
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            });

            services.AddSingleton<ISecureTokenOptions>(new SecureTokenOptions { SecureToken = Configuration["SecureToken"] });
            services.AddScoped<ISecureTokenAuthenticationHandler, SecureTokenAuthenticationHandler>();

            services.AddScoped<IUserTokenConfiguration, UserTokenConfiguration>();
            services.AddScoped<IUserTokenAuthenticationHandler, UserTokenAuthenticationHandler>();

            services.AddAuthentication(configure => {
                configure.AddScheme(SecureTokenSchema.Name, builder => {
                    builder.DisplayName = SecureTokenSchema.Description;
                    builder.HandlerType = typeof(ISecureTokenAuthenticationHandler);
                });
                configure.AddScheme(UserTokenSchema.Name, builder => {
                    builder.DisplayName = UserTokenSchema.Description;
                    builder.HandlerType = typeof(IUserTokenAuthenticationHandler);
                });
                configure.DefaultScheme = SecureTokenSchema.Name;
            }).AddBearerAuthentication(options => {
                options.Authority = Configuration["OAuthAuthority"];
                options.Audience = Configuration["OAuthAudience"];
            });

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine( new QueryStringApiVersionReader(), new HeaderApiVersionReader( "api-version" ));
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Authorization Heimdall (DocumentDB) API",
                    Description = "Microservice for Azure DocumentDB authorization for Bibliotheca.",
                    TermsOfService = "None"
                });

                var basePath = System.AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Bibliotheca.Server.Authorization.Heimdall.Api.xml"); 
                options.IncludeXmlComments(xmlPath);
            });

            services.AddNeutrinoClient(options => {
                options.SecureToken = Configuration["ServiceDiscovery:ServerSecureToken"];
                options.Addresses = Configuration.GetSection("ServiceDiscovery:ServerAddresses").GetChildren().Select(x => x.Value).ToArray();
            });

            services.AddScoped<IServiceDiscoveryRegistrationJob, ServiceDiscoveryRegistrationJob>();

            services.AddScoped<IUserTokenConfiguration, UserTokenConfiguration>();
            services.AddScoped<IUsersService, UsersService>();
        }

        /// <summary>
        /// Configure web application.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">Environment parameters.</param>
        /// <param name="loggerFactory">Logger.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if(env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            else
            {
                loggerFactory.AddAzureWebAppDiagnostics();
            }

            if (UseServiceDiscovery)
            {
                app.UseHangfireServer();
                RecurringJob.AddOrUpdate<IServiceDiscoveryRegistrationJob>("register-service", x => x.RegisterServiceAsync(null), Cron.Minutely);
            }

            app.UseExceptionHandler();
            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
            });
        }
    }
}
