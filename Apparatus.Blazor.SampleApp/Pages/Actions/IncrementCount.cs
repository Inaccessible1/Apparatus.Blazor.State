using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.Pages.Actions
{
    public class IncrementCount : IAction
    {
        public int IncrementBy { get; set; } = 0;
    }
}
