using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.States
{
    public class CounterState : IState
    {
        public int CurrentCount { get; set; }
    }
}
