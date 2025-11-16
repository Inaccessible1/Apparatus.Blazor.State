using Apparatus.Blazor.State.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Apparatus.Blazor.State
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Registers all implementations of a marker interface as singleton services.
        /// </summary>
        /// <typeparam name="TMarker">The marker interface type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="assemblies">The assemblies to scan for implementations.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddServicesByMarkerInterface<TMarker>(this IServiceCollection services, Assembly[] assemblies)
        {
            var interfaceType = typeof(TMarker);

            var implementingServices = assemblies.SelectMany(s => s.GetTypes())
                .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            foreach (var service in implementingServices)
            {
                services.AddSingleton(service);
            }

            return services;
        }

        private static IServiceCollection AddServicesByMarkerInterface<TMarker>(this IServiceCollection services, Assembly assembly)
        {
            return services.AddServicesByMarkerInterface<TMarker>([assembly]);
        }

        /// <summary>
        /// Registers all state and action handler services required for state management.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assemblies">The assemblies to scan for states and handlers.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddStateManagement(this IServiceCollection services, Assembly[] assemblies)
        {
            services.AddGenericTypeServices(assemblies, typeof(IActionHandler<>));
            services.AddServicesByMarkerInterface<IState>(assemblies);
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

        private static IServiceCollection AddGenericTypeServices(this IServiceCollection services, Assembly[] assemblies, Type genericServiceInterfaceType)
        {
            var implementations = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericServiceInterfaceType));

            foreach (var implementation in implementations)
            {
                var actualInterfaceType = implementation.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericServiceInterfaceType);

                services.AddScoped(actualInterfaceType, implementation);
            }

            return services;
        }
    }
}
