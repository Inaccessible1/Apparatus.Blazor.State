using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.Components.ConfirmationModal.Actions.Open
{
    public class OpenConfirmationModel : IAction
    {
        public string Content { get; set; }

        public IAction DispatchOnConfirmation { get; set; }
    }
}
