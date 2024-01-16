using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System;

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
                        config.AddAzureAppConfiguration(options =>
                        {
                            var connectionString = settings["ConnectionStrings:AppConfig"];
                            options.Connect(connectionString)
                                .ConfigureKeyVault(kv =>
                                {
                                    kv.SetCredential(new ManagedIdentityCredential());
                                })
                                .Select(KeyFilter.Any)
                                .Select(KeyFilter.Any, context.HostingEnvironment.EnvironmentName)
                                .ConfigureRefresh(refreshOptions =>
                                {
                                    refreshOptions.Register("Sentinel", true);
                                    refreshOptions.SetCacheExpiration(TimeSpan.FromSeconds(30));
                                });
                        });
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options => options.AddServerHeader = false);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
