using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Erabikata.Tests;

public class BackendFactory : WebApplicationFactory<Backend.Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Development")
            .ConfigureAppConfiguration(
                configurationBuilder => configurationBuilder.AddYamlFile("testSettings.yaml")
            );
    }
}
