

using Apparatus.Blazor.State.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Apparatus.Blazor.State
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddServicesByMarkerInterface<TMarker>(this IServiceCollection services, Assembly[] asssemblies)
        {
            var interfaceType = typeof(TMarker);

            var _services = asssemblies.SelectMany(s => s.GetTypes())
           .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            foreach (var serivce in _services)
                services.AddSingleton(serivce);

            return services;
        }

        private static IServiceCollection AddServicesByMarkerInterface<TMarker>(this IServiceCollection services, Assembly assembly)
        {
            return services.AddServicesByMarkerInterface<TMarker>([assembly]);
        }

        //private static IServiceCollection AddServicesByMarkerInterface<TMarker>(this IServiceCollection services)
        //{
        //    return services.AddServicesByMarkerInterface<TMarker>(typeof(TMarker).Assembly);
        //}

        public static IServiceCollection AddStateManagement(this IServiceCollection services, Assembly[] asssemblies)
        {
            services.AddGenericTypeServices(asssemblies, typeof(IActionHandler<>));
            services.AddServicesByMarkerInterface<IState>(asssemblies);
            services.AddScoped<IActionDispatcher, ActionDispatcher>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IActionSubscriber, ActionSubscriber>();
            services.AddScoped(typeof(IStore<>), typeof(Store<>));

            return services;
        }

        public static IServiceCollection AddStateManagement(this IServiceCollection services, Assembly assembly)
        {
            return services.AddStateManagement([assembly]);
        }

        private static IServiceCollection AddGenericTypeServices(this IServiceCollection services, Assembly[] asssemblies, Type genericServiceInterfaceType)
        {
            var implementations = asssemblies.SelectMany(a => a.GetTypes())
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
