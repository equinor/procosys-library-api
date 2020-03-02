using Equinor.Procosys.Library.Command.EventHandlers;
using Equinor.Procosys.Library.Domain;
using Equinor.Procosys.Library.Domain.Events;
using Equinor.Procosys.Library.Infrastructure;
using Equinor.Procosys.Library.WebApi.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.Procosys.Library.WebApi.DIModules
{
    public static class ApplicationModule
    {
        public static void AddApplicationModules(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LibraryContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("LibraryContext"));
            });

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            // Transient - Created each time it is requested from the service container


            // Scoped - Created once per client request (connection)
            services.AddScoped<IPlantProvider, PlantProvider>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IUnitOfWork>(x => x.GetRequiredService<LibraryContext>());
            services.AddScoped<IReadOnlyContext>(x => x.GetRequiredService<LibraryContext>());

            // Singleton - Created the first time they are requested
            services.AddSingleton<ITimeService, TimeService>();
        }
    }
}
