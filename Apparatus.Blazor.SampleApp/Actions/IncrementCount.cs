using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.Actions
{
    public class IncrementCount : IAction
    {
        public int IncrementBy { get; set; }
    }
}
