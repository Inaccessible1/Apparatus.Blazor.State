using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.Components.ConfirmationModal
{
    public class ConfirmationModalState : IState
    {
        public bool Display { get; set; } = false;

        public string Content { get; set; }

        public IAction DispatchOnConfirmation { get; set; }
    }
}
