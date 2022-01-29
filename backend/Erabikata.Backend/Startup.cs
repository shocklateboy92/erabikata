using System;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.CollectionMiddlewares;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models;
using Erabikata.Backend.Models.Database;
using Erabikata.Models.Configuration;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using MongoDB.Driver;
using Newtonsoft.Json;
using NJsonSchema.Generation;
using Refit;

namespace Erabikata.Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddSnapshotCollector();

            services.Configure<SubtitleProcessingSettings>(
                Configuration.GetSection("SubtitleProcessing")
            );
            services.Configure<VideoInputSettings>(Configuration.GetSection("VideoInput"));

            ConfigureDatabase(services);

            services.AddCollectionManagers();
            services.AddCollectionMiddlewares();

            services.AddSingleton<SeedDataProvider>();

            services.AddCors(
                options =>
                    options.AddDefaultPolicy(
                        builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                    )
            );

            AddGrpcClient<AnalyzerService.AnalyzerServiceClient>(services);
            AddGrpcClient<AssParserService.AssParserServiceClient>(services);

            services.AddMicrosoftIdentityWebApiAuthentication(Configuration);

            services
                .AddControllers()
                .AddNewtonsoftJson(
                    options =>
                    {
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    }
                );

            services.AddSpaStaticFiles(
                options =>
                {
                    options.RootPath = "wwwroot";
                }
            );

            services.AddOpenApiDocument(
                settings =>
                {
                    settings.GenerateKnownTypes = true;
                    settings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
                    settings.RequireParametersWithoutDefault = true;
                    settings.SerializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new JsonContractResolver(settings)
                    };
                }
            );
        }

        private IHttpClientBuilder AddGrpcClient<TServicer>(IServiceCollection services)
            where TServicer : class
        {
            return services.AddGrpcClient<TServicer>(
                options =>
                {
                    options.Address = new Uri(
                        Configuration.GetSection("ServiceClients:Analyzer:BaseUrl").Value
                    );
                }
            );
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

            var url = new MongoUrl(connectionString);
            var clientSettings = MongoClientSettings.FromConnectionString(connectionString);

            // var dbLogger = services.BuildServiceProvider()
            //     .GetRequiredService<ILogger<MongoClient>>();
            // clientSettings.ClusterConfigurator = cb =>
            // {
            //     cb.Subscribe<CommandStartedEvent>(
            //         e =>
            //         {
            //             dbLogger.LogInformation(
            //                 "{CommandName} - {Command}",
            //                 e.CommandName,
            //                 e.Command.ToJson(new JsonWriterSettings {Indent = true})
            //             );
            //         }
            //     );
            // };

            var mongoDatabase = new MongoClient(clientSettings).GetDatabase(url.DatabaseName);
            services.AddSingleton(mongoDatabase);
            AddCollection<ActivityExecution>(services, mongoDatabase);
            AddCollection<WordState>(services, mongoDatabase);
            AddCollection<UserInfo>(services, mongoDatabase);

            services
                .AddRefitClient<IAnkiSyncClient>()
                .ConfigureHttpClient(
                    client =>
                    {
                        client.BaseAddress = new Uri(
                            Configuration.GetSection("ServiceClients:Anki:BaseUrl").Value
                        );
                    }
                );
        }

        private static void AddCollection<TDataType>(
            IServiceCollection services,
            IMongoDatabase mongoDatabase
        )
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

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                }
            );

            app.UseStaticFiles();
            app.UseWhen(
                context =>
                    !context.Request.Path.Value?.StartsWith(
                        "/api",
                        StringComparison.OrdinalIgnoreCase
                    ) ?? false,
                appBuilder =>
                    appBuilder.UseSpa(
                        builder =>
                        {
                            builder.Options.DefaultPage = "/index.html";
                            if (env.IsDevelopment())
                            {
                                builder.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                            }
                        }
                    )
            );
        }
    }
}
