using System;
using System.Reflection;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Erabikata.Backend
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // await host.Services.GetRequiredService<WordCountsManager>().Initialize();
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (context, config) =>
                    {
                        config.AddJsonFile($"appsettings.{Environment.MachineName}.json", true);
                    }
                )
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}