using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Erabikata.Tests
{
    public class Startup
    {
        public void ConfigureHost(IHostBuilder hostBuilder) =>
            hostBuilder.UseEnvironment("Development")
                .ConfigureWebHostDefaults(
                    webHostBuilder => webHostBuilder.UseTestServer()
                        .UseStartup<Erabikata.Backend.Startup>()
                        .ConfigureLogging(
                            (builder, logging) =>
                            {
                                logging.AddConsole();
                                logging.AddConfiguration(
                                    builder.Configuration.GetSection("Logging")
                                );
                            }
                        )
                        .ConfigureAppConfiguration(
                            builder => builder.AddJsonFile("appsettings.json")
                                .AddJsonFile("appsettings.Development.json")
                                .AddYamlFile("testSettings.yaml")
                        )
                );
    }
}
