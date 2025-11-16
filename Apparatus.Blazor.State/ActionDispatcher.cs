using Apparatus.Blazor.State.Contracts;
using Microsoft.Extensions.Logging;
using System.Reflection;

#nullable enable
namespace Apparatus.Blazor.State
{
    public class ActionDispatcher : IActionDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IActionSubscriber _actionSubscriber;
        private readonly ILogger<ActionDispatcher>? _logger;
        private static readonly MethodInfo? _cachedGenericDispatchMethod;

        static ActionDispatcher()
        {
            // Cache the generic Dispatch method at startup to avoid reflection overhead
            _cachedGenericDispatchMethod = typeof(ActionDispatcher)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(m => m.Name == nameof(Dispatch) && m.IsGenericMethod && m.GetGenericArguments().Length == 1);
        }

        public ActionDispatcher(IServiceProvider serviceProvider, IActionSubscriber actionSubscriber, ILogger<ActionDispatcher>? logger = null)
        {
            _serviceProvider = serviceProvider;
            _actionSubscriber = actionSubscriber;
            _logger = logger;
        }

        public async Task Dispatch<TAction>(TAction action) where TAction : IAction
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                _logger?.LogDebug("Dispatching action {ActionType}", typeof(TAction).Name);

                // Publish action to subscribers first
                _actionSubscriber.Publish(action);

                // Then handle if a handler exists
                if (_serviceProvider.GetService(typeof(IActionHandler<TAction>)) is IActionHandler<TAction> handler)
                {
                    await handler.Handle(action);
                    _logger?.LogDebug("Action {ActionType} handled successfully", typeof(TAction).Name);
                }
                else
                {
                    _logger?.LogDebug("No handler registered for action {ActionType}", typeof(TAction).Name);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error dispatching action {ActionType}: {ErrorMessage}", typeof(TAction).Name, ex.Message);
                throw new ActionDispatchException($"Failed to dispatch action {typeof(TAction).Name}", ex);
            }
        }

        public async Task Dispatch(IAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                var actionType = action.GetType();
                
                if (_cachedGenericDispatchMethod == null)
                {
                    throw new InvalidOperationException($"Cannot find generic Dispatch method for action type {actionType.Name}");
                }

                var genericMethod = _cachedGenericDispatchMethod.MakeGenericMethod(actionType);
                var task = (Task?)genericMethod.Invoke(this, new object[] { action });
                
                if (task != null)
                {
                    await task;
                }
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                // Unwrap TargetInvocationException to preserve the original exception
                throw ex.InnerException;
            }
        }
    }
}
