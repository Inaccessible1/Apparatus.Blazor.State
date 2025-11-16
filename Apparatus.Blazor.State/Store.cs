using Apparatus.Blazor.State.Contracts;
using Microsoft.Extensions.Logging;

namespace Apparatus.Blazor.State
{
    public class Store<TState> : IStore<TState> where TState : class, IState
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<Store<TState>>? _logger;

        public Store(TState state, ISubscriptionService subscriptionService, ILogger<Store<TState>>? logger = null)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public TState State { get; private set; }

        public Task SetState(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (ReferenceEquals(State, state))
            {
                _logger?.LogWarning("State object is the same reference. Consider using immutable state objects or cloning state before modifications to prevent unintended side effects.");
            }

            _logger?.LogDebug("Setting new state for {StateType}", typeof(TState).Name);
            
            State = state;

            return Refresh();
        }

        public Task Refresh()
        {
            _logger?.LogDebug("Refreshing subscribers for {StateType}", typeof(TState).Name);
            
            _subscriptionService.ReRenderSubscribers<TState>();

            return Task.CompletedTask;
        }
    }
}
