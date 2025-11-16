namespace Apparatus.Blazor.State.Contracts
{
    /// <summary>
    /// Represents a Blazor component that participates in state management.
    /// </summary>
    public interface IBlazorStateComponent
    {
        /// <summary>
        /// Gets the unique identifier of this component.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the state of the specified type, registering this component as a subscriber.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <returns>The state instance, or a new instance if not registered in the service provider.</returns>
        TState GetState<TState>() where TState : IState, new();

        /// <summary>
        /// Triggers a re-render of this component.
        /// </summary>
        void ReRender();
    }
}
