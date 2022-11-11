using Apparatus.Blazor.SampleApp.Actions;
using Apparatus.Blazor.SampleApp.States;
using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.Handlers
{
    public class RemoveTableRowHandler : IActionHandler<RemoveTableRow>
    {
        private IStore<WeatherForecastTableState> _tableStore; 

        public RemoveTableRowHandler(IStore<WeatherForecastTableState> tableStore)
        {
            _tableStore = tableStore;
        }

        public Task Handle(RemoveTableRow action)
        {
            var state = _tableStore.State;

            state.Forecasts = state.Forecasts.Where(f => f.Date != action.Date).ToList();

            return _tableStore.SetState(state);
        }
    }
}
