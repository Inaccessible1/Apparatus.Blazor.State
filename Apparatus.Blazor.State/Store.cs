using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.State
{
    public class Store<TState> : IStore<TState> where TState : class, IState
    {
        private TState _state;
        private readonly ISubscriptionService _subscriptionService;

        public Store(TState state, ISubscriptionService subscriptionService)
        {
            State = state;
            _state = state;
            _subscriptionService = subscriptionService;
        }

        public TState State { get; private set; }

        public Task SetState(TState state)
        {
            _state = state;
            State = state;

            return Refresh();
        }

        public Task Refresh()
        {
            _subscriptionService.ReRenderSubscribers<TState>();

            return Task.CompletedTask;
        }
    }
}
