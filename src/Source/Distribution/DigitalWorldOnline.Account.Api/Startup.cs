using AspNetCoreRateLimit;
using DigitalWorldOnline.Api.Dtos.Errors;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Application.Admin.Repositories;
using DigitalWorldOnline.Application.Extensions;
using DigitalWorldOnline.Application.Services;
using DigitalWorldOnline.Commons.Interfaces;
using DigitalWorldOnline.Commons.Repositories.Admin;
using DigitalWorldOnline.Infrastructure;
using DigitalWorldOnline.Infrastructure.Mapping;
using DigitalWorldOnline.Infrastructure.Repositories.Account;
using DigitalWorldOnline.Infrastructure.Repositories.Admin;
using DigitalWorldOnline.Infrastructure.Repositories.Character;
using DigitalWorldOnline.Infrastructure.Repositories.Config;
using DigitalWorldOnline.Infrastructure.Repositories.Routine;
using DigitalWorldOnline.Infrastructure.Repositories.Server;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Net;
using System.Reflection;

namespace DigitalWorldOnline.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration["Database:Connection"]));
            services.AddScoped<IAdminQueriesRepository, AdminQueriesRepository>();
            services.AddScoped<IAdminCommandsRepository, AdminCommandsRepository>();

            services.AddScoped<IServerQueriesRepository, ServerQueriesRepository>();
            services.AddScoped<IServerCommandsRepository, ServerCommandsRepository>();

            services.AddScoped<IAccountQueriesRepository, AccountQueriesRepository>();
            services.AddScoped<IAccountCommandsRepository, AccountCommandsRepository>();

            services.AddScoped<ICharacterQueriesRepository, CharacterQueriesRepository>();
            services.AddScoped<ICharacterCommandsRepository, CharacterCommandsRepository>();

            services.AddScoped<IConfigQueriesRepository, ConfigQueriesRepository>();
            services.AddScoped<IConfigCommandsRepository, ConfigCommandsRepository>();

            services.AddScoped<IRoutineRepository, RoutineRepository>();
            
            services.AddScoped<IEmailService, EmailService>();

            services.AddTransient<Mediator>();
            services.AddSingleton<ISender, ScopedSender<Mediator>>();
            services.AddMediatR(typeof(MediatorApplicationHandlerExtension).GetTypeInfo().Assembly);
            services.AddSingleton(ConfigureLogger(Configuration));
            services.AddMemoryCache();
            services.AddControllers();
            services.AddValidatorsFromAssemblyContaining<CreateUserAccountCommandValidator>(ServiceLifetime.Transient);
            services.AddEndpointsApiExplorer();
            services.AddAuthentication("Bearer");
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api Digital Master", Description = "Digital Master - API", Version = "v1" });
            });

            //services.AddOptions();
            //services.AddMemoryCache();
            //services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            //services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));
            //services.AddInMemoryRateLimiting();
            //services.AddMvc();
            //services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            AddAutoMapper(services);
        }

        private static void AddAutoMapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AccountProfile));
            services.AddAutoMapper(typeof(AssetsProfile));
            services.AddAutoMapper(typeof(CharacterProfile));
            services.AddAutoMapper(typeof(ConfigProfile));
            services.AddAutoMapper(typeof(DigimonProfile));
            services.AddAutoMapper(typeof(GameProfile));
            services.AddAutoMapper(typeof(SecurityProfile));
        }

        static Serilog.ILogger ConfigureLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(configuration["Log:DebugRepository"] ?? "logs\\Debug\\Api", Serilog.Events.LogEventLevel.Debug, retainedFileCountLimit: 100)
                .WriteTo.RollingFile(configuration["Log:InformationRepository"] ?? "logs\\Information\\Api", Serilog.Events.LogEventLevel.Information, retainedFileCountLimit: 100)
                .WriteTo.RollingFile(configuration["Log:WarningRepository"] ?? "logs\\Warning\\Api", Serilog.Events.LogEventLevel.Warning, retainedFileCountLimit: 100)
                .WriteTo.RollingFile(configuration["Log:ErrorRepository"] ?? "logs\\Error\\Api", Serilog.Events.LogEventLevel.Error, retainedFileCountLimit: 100)
                .CreateLogger();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DigitalWorldOnline.Api v1"));
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DigitalWorldOnline.Api v1"));

                app.UseExceptionHandler(appError =>
                {
                    appError.Run(async context =>
                    {
                        context.Response.ContentType = "application/json";
                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                        if (contextFeature != null)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                code = 9999,
                                error = "Internal Server Error"
                            }.ToString());
                        }
                    });
                });
            }

            //app.UseClientRateLimiting();
            //app.UseMvc();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
