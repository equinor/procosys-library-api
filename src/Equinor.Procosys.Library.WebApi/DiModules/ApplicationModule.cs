using Equinor.Procosys.Library.Domain.Time;
using Equinor.Procosys.Library.Query.Client;
using Equinor.Procosys.Library.WebApi.Misc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.Procosys.Library.WebApi.DIModules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiOptions>(configuration.GetSection("API"));
            services.Configure<MainApiOptions>(configuration.GetSection("MainApi"));

            //services.AddDbContext<LibraryContext>(options =>
            //{
            //    options.UseSqlServer(configuration.GetConnectionString("LibraryContext"));
            //});

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // Transient - Created each time it is requested from the service container


            // Scoped - Created once per client request (connection)
            services.AddScoped<IBearerTokenApiClient, BearerTokenApiClient>();
            services.AddScoped<IBearerTokenProvider, MainApiBearerTokenProvider>();
            //services.AddScoped<IEventDispatcher, EventDispatcher>();
            //services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<LibraryContext>());
            //services.AddScoped<IReadOnlyContext>(x => x.GetRequiredService<LibraryContext>());

            // Singleton - Created the first time they are requested


            TimeService.SetProvider(new SystemTimeProvider());
        }
    }
}
