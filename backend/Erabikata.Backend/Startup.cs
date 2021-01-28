using System;
using System.Text.Json.Serialization;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Managers;
using Erabikata.Backend.Models.Actions;
using Erabikata.Models.Configuration;
using JsonSubTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using MongoDB.Driver;
using Action = Erabikata.Backend.Models.Actions.Action;

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

            var connectionString = Configuration.GetSection("Db:ConnectionString").Get<string>();
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Db:ConnectionString not specified. Unable to startup."
                );
            }

            services.AddSingleton(_ => new MongoClient(connectionString).GetDatabase("erabikata"));
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

            services.AddOpenApiDocument();
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
            app.UseSwaggerUi3();

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