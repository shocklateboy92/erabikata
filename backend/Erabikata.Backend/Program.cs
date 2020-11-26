using System;
using System.Threading.Tasks;
using Erabikata.Backend.Managers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Erabikata.Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await host.Services.GetRequiredService<SubtitleDatabaseManager>().Initialize();

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (context, config) =>
                    {
                        config.AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true);

                        var keyVaultName = config.Build()["KeyVaultName"];
                        if (string.IsNullOrWhiteSpace(keyVaultName))
                        {
                            return;
                        }

                        Console.WriteLine("Attempting to connect KeyVault " + keyVaultName);

                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback
                            )
                        );

                        config.AddAzureKeyVault(
                            $"https://{keyVaultName}.vault.azure.net/",
                            keyVaultClient,
                            new DefaultKeyVaultSecretManager()
                        );
                    }
                )
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}
