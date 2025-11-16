using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Apparatus.Blazor.State.Test")]
namespace Apparatus.Blazor.State.Contracts
{
    /// <summary>
    /// Service for subscribing to and publishing actions.
    /// </summary>
    public interface IActionSubscriber
    {
        /// <summary>
        /// Subscribes a handler to be notified when an action is published.
        /// </summary>
        /// <typeparam name="TAction">The action type.</typeparam>
        /// <param name="handler">The handler to invoke when the action is published.</param>
        /// <returns>A subscription identifier that can be used to unsubscribe later.</returns>
        string Subscribe<TAction>(Action<TAction> handler) where TAction : IAction;

        /// <summary>
        /// Unsubscribes a handler from a specific action subscription.
        /// </summary>
        /// <typeparam name="TAction">The action type.</typeparam>
        /// <param name="subscriptionId">The subscription identifier obtained during Subscribe.</param>
        void Unsubscribe<TAction>(string subscriptionId) where TAction : IAction;

        internal void Publish<TAction>(TAction action) where TAction : IAction;
    }
}
