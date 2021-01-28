using System;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Controllers;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Managers;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Erabikata.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using MongoDB.Driver;

namespace Erabikata.Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SubtitleProcessingSettings>(
                Configuration.GetSection("SubtitleProcessing")
            );
            services.Configure<VideoInputSettings>(Configuration.GetSection("VideoInput"));

            ConfigureDatabase(services);

            services.AddSingleton<ICollectionManager, WordStateManager>();
            services.AddSingleton<ICollectionManager, DummyCollectionManager>();

            services.AddSingleton<SubtitleDatabaseManager>();
            services.AddSingleton<WordCountsManager>();
            services.AddSingleton<EpisodeInfoManager>();

            services.AddHttpClient<KnownWordsProvider>();

            services.AddCors(
                options => options.AddDefaultPolicy(
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                )
            );

            services.AddMicrosoftIdentityWebApiAuthentication(Configuration);

            services.AddControllers().AddNewtonsoftJson();

            services.AddSpaStaticFiles(options => { options.RootPath = "wwwroot"; });

            services.AddOpenApiDocument(settings => { settings.GenerateKnownTypes = true; });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            var connectionString = Configuration.GetSection("Db:ConnectionString").Get<string>();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Db:ConnectionString not specified. Unable to startup."
                );
            }

            var mongoDatabase = new MongoClient(connectionString).GetDatabase("erabikata");
            AddCollection<ActivityExecution>(services, mongoDatabase);
            AddCollection<WordState>(services, mongoDatabase);
            AddCollection<UserInfo>(services, mongoDatabase);
        }

        private static void AddCollection<TDataType>(
            IServiceCollection services,
            IMongoDatabase mongoDatabase)
        {
            services.AddSingleton(mongoDatabase.GetCollection<TDataType>(typeof(TDataType).Name));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseReDoc();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseStaticFiles();
            app.UseSpa(
                builder =>
                {
                    builder.Options.DefaultPage = "/index.html";
                    if (env.IsDevelopment())
                    {
                        builder.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                    }
                }
            );
        }
    }
}