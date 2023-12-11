using Apparatus.Blazor.SampleApp.Components.ConfirmationModal.Actions.Close;
using Apparatus.Blazor.State.Contracts;
using Microsoft.AspNetCore.Components;

#nullable disable annotations
namespace Apparatus.Blazor.SampleApp.Components.ConfirmationModal
{
    public partial class ConfirmationModal
    {
        [Inject] IActionDispatcher Dispatcher { get; set; }

        ConfirmationModalState State => GetState<ConfirmationModalState>();

        public Task Hide()
        {
            return Dispatcher.Dispatch(new CloseConfirmationModal());
        }

        public async Task OnConfirm()
        {
            await Dispatcher.Dispatch(State.DispatchOnConfirmation);

            await Hide();
        }
    }
}
