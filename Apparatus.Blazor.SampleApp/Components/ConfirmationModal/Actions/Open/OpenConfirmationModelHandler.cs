using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.Components.ConfirmationModal.Actions.Open
{
    public class OpenConfirmationModelHandler : IActionHandler<OpenConfirmationModel>
    {
        private IStore<ConfirmationModalState> _store;

        public OpenConfirmationModelHandler(IStore<ConfirmationModalState> store)
        {
            _store = store;
        }

        public async Task Handle(OpenConfirmationModel action)
        {
            var state = _store.State;

            state.Content = action.Content;
            state.DispatchOnConfirmation = action.DispatchOnConfirmation;
            state.Display = true;

            await _store.SetState(state);
        }
    }
}
