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

            /*
             or use action property for increment value
             newState.CurrentCount = newState.CurrentCount + action.IncrementBy;
             */

            return _counterStore.SetState(newState); //SetState will set new state and trigger re-rendering of all blazor components which are accessing CounterState via GetState<CounterState>() method.
        }
    }
}
