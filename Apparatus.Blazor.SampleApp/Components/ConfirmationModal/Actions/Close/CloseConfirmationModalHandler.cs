using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.SampleApp.Components.ConfirmationModal.Actions.Close
{
    public class CloseConfirmationModalHandler : IActionHandler<CloseConfirmationModal>
    {
        private IStore<ConfirmationModalState> _store;

        public CloseConfirmationModalHandler(IStore<ConfirmationModalState> store)
        {
            _store = store; 
        }
        public Task Handle(CloseConfirmationModal action)
        {
            var state = _store.State;

            state.Display = false;

            return _store.SetState(state);
        }
    }
}
