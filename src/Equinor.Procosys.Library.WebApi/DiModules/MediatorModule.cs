using System.Reflection;
using Equinor.Procosys.Library.Command;
using Equinor.Procosys.Library.Query;
using Equinor.Procosys.Library.WebApi.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.Procosys.Library.WebApi.DIModules
{
    public static class MediatorModule
    {
        public static void AddMediatrModules(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(MediatorModule).GetTypeInfo().Assembly,
                    typeof(ICommandMarker).GetTypeInfo().Assembly,
                    typeof(IQueryMarker).GetTypeInfo().Assembly
                );

            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        }
    }
}
