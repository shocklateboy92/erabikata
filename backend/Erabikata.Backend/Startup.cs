using System;
using System.Linq;
using System.Text.Json.Serialization;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.CollectionMiddlewares;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models;
using Erabikata.Backend.Models.Configuration;
using Erabikata.Backend.Models.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Refit;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Erabikata.Backend;

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
            .AddJsonOptions(
                options =>
                {
                    var settings = options.JsonSerializerOptions;
                    settings.Converters.Add(new JsonStringEnumConverter());
                    settings.Converters.Add(new ActivityJsonConverter());
                    settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                }
            );
        services.AddSpaStaticFiles(
            options =>
            {
                options.RootPath = "wwwroot";
            }
        );

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
            options =>
            {
                options.SupportNonNullableReferenceTypes();
                options.UseOneOfForPolymorphism();
                options.SchemaFilter<RequiredNotNullableSchemaFilter>();
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

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseEndpoints(
            endpoints =>
            {
                endpoints.MapControllers();
            }
        );

        app.UseStaticFiles();
        app.UseWhen(
            context =>
                !context.Request.Path.Value?.StartsWith("/api", StringComparison.OrdinalIgnoreCase)
                ?? false,
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

/// <summary>
/// Most TypeScript code generators for OpenApi generate properties as optional
/// unless they're marked as `required` in the model. Because C# has no concept
/// of optional properties, all Non-Nullable properties should be `required`.
///
/// This is a schema filter that marks does exactly that.
/// </summary>
public class RequiredNotNullableSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
        {
            return;
        }

        var notNullableProperties = schema.Properties
            .Where(x => !x.Value.Nullable && !schema.Required.Contains(x.Key))
            .ToList();

        foreach (var property in notNullableProperties)
        {
            schema.Required.Add(property.Key);
        }
    }
}
