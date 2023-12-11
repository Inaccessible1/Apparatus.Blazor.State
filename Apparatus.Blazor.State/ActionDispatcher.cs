using Apparatus.Blazor.State.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (_serviceProvider.GetService(typeof(IActionHandler<TAction>)) is IActionHandler<TAction> handler)
            {
                await handler.Handle(action);

                await Task.Delay(1);
            }

            _actionSubscriber.Publish(action);

            await Task.Delay(1);
        }

        public async Task Dispatch(IAction action)
        {
            _actionSubscriber.Publish(action);
            await Task.Delay(1);

            var actionType = action.GetType();
            var handlerType = typeof(IActionHandler<>).MakeGenericType(actionType);

            if (handlerType != null)
            {
                dynamic handler = _serviceProvider.GetService(handlerType);

                if (handler != null)
                    await handler.Handle((dynamic)action);
                
            }
        }
    }
}
