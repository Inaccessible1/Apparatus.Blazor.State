namespace Apparatus.Blazor.State.Contracts
{
    /// <summary>
    /// Manages subscriptions of Blazor components to state changes.
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Triggers re-rendering of all components subscribed to a specific state type.
        /// </summary>
        /// <param name="stateType">The state type.</param>
        void ReRenderSubscribers(Type stateType);

        /// <summary>
        /// Triggers re-rendering of all components subscribed to a specific state type.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        void ReRenderSubscribers<TState>() where TState : IState;

        /// <summary>
        /// Subscribes a component to state changes.
        /// </summary>
        /// <param name="stateType">The state type to subscribe to.</param>
        /// <param name="blazorStateComponent">The component to subscribe.</param>
        void Add(Type stateType, IBlazorStateComponent blazorStateComponent);

        /// <summary>
        /// Unsubscribes a component from all state changes.
        /// </summary>
        /// <param name="blazorStateComponent">The component to unsubscribe.</param>
        void Remove(IBlazorStateComponent blazorStateComponent);
    }
}
