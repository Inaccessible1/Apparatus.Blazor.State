using Apparatus.Blazor.State.Contracts;

#nullable disable annotations
namespace Apparatus.Blazor.State
{
    public class ActionDispatcher : IActionDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IActionSubscriber _actionSubscriber;

        public ActionDispatcher(IServiceProvider serviceProvider, IActionSubscriber actionSubscriber)
        {
            _serviceProvider = serviceProvider;
            _actionSubscriber = actionSubscriber;
        }

        public async Task Dispatch<TAction>(TAction action) where TAction : IAction
        {
            // Publish action to subscribers first
            _actionSubscriber.Publish(action);

            // Then handle if a handler exists
            if (_serviceProvider.GetService(typeof(IActionHandler<TAction>)) is IActionHandler<TAction> handler)
            {
                await handler.Handle(action);
            }
        }

        public async Task Dispatch(IAction action)
        {
            // Delegate to generic version for consistency
            var actionType = action.GetType();
            var genericDispatchMethod = typeof(ActionDispatcher)
                .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(m => m.Name == nameof(Dispatch) && m.IsGenericMethod && m.GetGenericArguments().Length == 1)
                .FirstOrDefault()
                ?.MakeGenericMethod(actionType)
                ?? throw new InvalidOperationException($"Cannot find generic Dispatch method for action type {actionType.Name}");

            await (Task)genericDispatchMethod.Invoke(this, [action])!;
        }
    }
}
