using Apparatus.Blazor.State.Contracts;
using Microsoft.AspNetCore.Components;

#nullable enable
namespace Apparatus.Blazor.State
{
    /// <summary>
    /// Base class for Blazor components that participate in centralized state management.
    /// Components inheriting from this class can automatically subscribe to and react to state changes.
    /// </summary>
    /// <example>
    /// <code>
    /// @inherits BlazorStateComponent
    /// 
    /// @code {
    ///     private CounterState State => GetState&lt;CounterState&gt;();
    ///     
    ///     [Inject] IActionDispatcher Dispatcher { get; set; }
    ///     
    ///     private async Task IncrementCount()
    ///     {
    ///         await Dispatcher.Dispatch(new IncrementCountAction());
    ///     }
    /// }
    /// </code>
    /// </example>
    public class BlazorStateComponent : ComponentBase, IDisposable, IBlazorStateComponent
    {
        private static int _instanceCount = 0;

        public BlazorStateComponent()
        {
            string name = GetType().Name;
            int instanceId = Interlocked.Increment(ref _instanceCount);

            Id = $"{name}-{instanceId}";
        }

        [Inject] 
        public IServiceProvider ServiceProvider { get; set; } = default!;
        
        [Inject] 
        public ISubscriptionService SubscriptionService { get; set; } = default!;

        /// <summary>
        /// Gets the unique identifier of this component instance.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Triggers a re-render of this component by calling StateHasChanged.
        /// </summary>
        public void ReRender() => base.InvokeAsync(StateHasChanged);

        /// <summary>
        /// Disposes this component and removes it from all state subscriptions.
        /// </summary>
        public virtual void Dispose()
        {
            SubscriptionService?.Remove(this);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the state of the specified type, registering this component as a subscriber.
        /// </summary>
        /// <typeparam name="TState">The state type implementing IState.</typeparam>
        /// <returns>The state instance from the service provider, or a new default instance if not registered.</returns>
        /// <remarks>
        /// This method automatically registers the component as a subscriber to the specified state type.
        /// When the state is updated via SetState, this component will be notified to re-render.
        /// </remarks>
        public TState GetState<TState>() where TState : IState, new()
        {
            if (ServiceProvider.GetService(typeof(TState)) is TState state)
            {
                SubscriptionService.Add(typeof(TState), this);

                return state;
            }

            return new TState();
        }
    }
}
