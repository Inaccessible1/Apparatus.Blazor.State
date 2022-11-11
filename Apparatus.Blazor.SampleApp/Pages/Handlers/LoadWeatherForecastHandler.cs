using Apparatus.Blazor.SampleApp.Pages.Actions;
using Apparatus.Blazor.SampleApp.States;
using Apparatus.Blazor.State.Contracts;
using System.Net.Http.Json;


namespace Apparatus.Blazor.SampleApp.Pages.Handlers
{
    public class LoadWeatherForecastHandler : IActionHandler<LoadWeatherForecast>
    {
        private IStore<WeatherForecastTableState> _tableStore;
        private HttpClient _httpClient;

        public LoadWeatherForecastHandler(IStore<WeatherForecastTableState> tableStore, HttpClient httpClien)
        {
            _tableStore = tableStore;
            _httpClient = httpClien;
        }
        public async Task Handle(LoadWeatherForecast action)
        {
            var newState = _tableStore.State;

            newState.Loading = true;

            await _tableStore.SetState(newState);

            newState.Forecasts = await _httpClient.GetFromJsonAsync<List<WeatherForecast>>("sample-data/weather.json") ?? new();

            newState.Loading = false;

            newState.Initialized = true;

            await _tableStore.SetState(newState);
        }
    }
}
