using Apparatus.Blazor.SampleApp.Actions;
using Apparatus.Blazor.SampleApp.States;
using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.Handlers
{
    public class IncrementCountHandler : IActionHandler<IncrementCount>
    {
        private IStore<CounterState> _counterStore;

        public IncrementCountHandler(IStore<CounterState> counterStore)
        {
            _counterStore = counterStore; 
        }

        public Task Handle(IncrementCount action)
        {
            var newState = _counterStore.State;

            newState.CurrentCount++;

            return _counterStore.SetState(newState);
        }
    }
}
