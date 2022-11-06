

using Apparatus.Blazor.State.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Apparatus.Blazor.State
{
    public static class ServiceCollectionExtension
    {
        private static IServiceCollection AddServicesByMarkerInterface<TMarker>(this IServiceCollection services, Assembly[] asssemblies)
        {
            var interfaceType = typeof(TMarker);

            var _services = asssemblies.SelectMany(s => s.GetTypes())
           .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            foreach (var serivce in _services)
                services.AddSingleton(serivce);

            return services;
        }

        private static IServiceCollection AddServicesByMarkerInterface<TMarker>(this IServiceCollection services, Assembly asssembly)
        {
            return services.AddServicesByMarkerInterface<TMarker>(new Assembly[] { asssembly });
        }

        //private static IServiceCollection AddServicesByMarkerInterface<TMarker>(this IServiceCollection services)
        //{
        //    return services.AddServicesByMarkerInterface<TMarker>(typeof(TMarker).Assembly);
        //}

        public static IServiceCollection AddStateManagement(this IServiceCollection services, Assembly assembly)
        {
            services.AddGenericTypeServices(assembly, typeof(IActionHandler<>));
            services.AddServicesByMarkerInterface<IState>(assembly);
            services.AddScoped<IActionDispatcher, ActionDispatcher>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IActionSubscriber, ActionSubscriber>();
            services.AddScoped(typeof(IStore<>), typeof(Store<>));

            return services;
        }


        private static IServiceCollection AddGenericTypeServices(this IServiceCollection services, Assembly assembly, Type genericServiceInterfaceType)
        {
            var implementations = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericServiceInterfaceType)
            );

            foreach (var implementation in implementations)
            {
                var actualInterfaceType = implementation.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericServiceInterfaceType);

                services.AddScoped(actualInterfaceType, implementation);
            }

            return services;
        }
    }

}
