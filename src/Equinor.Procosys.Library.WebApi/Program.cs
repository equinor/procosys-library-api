using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Equinor.Procosys.Library.WebApi.Misc;

namespace Equinor.Procosys.Library.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var settings = config.Build();
                    var azConfig = settings.GetValue<bool>("UseAzureAppConfiguration");
                    if (azConfig)
                    {
                        var kvSettings = new KeyVaultSettings();
                        settings.GetSection("KeyVault").Bind(kvSettings);
                        if (kvSettings.Enabled)
                        {
                            config.AddAzureKeyVault(kvSettings.Uri, kvSettings.ClientId, kvSettings.ClientSecret);
                        }
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
