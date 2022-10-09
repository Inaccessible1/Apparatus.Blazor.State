using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.States
{
    public class WeatherForecastTableState : IState
    {
        public List<WeatherForecast> Forecasts { get; set; } = new();

        public bool Loading { get; set; }

        public bool Initialized { get; set; }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string? Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
