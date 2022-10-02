using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apparatus.Blazor.State.Contracts
{
    public interface ISubscriptionService
    {
        void ReRenderSubscribers(Type stateType);

        void ReRenderSubscribers<TState>() where TState : IState;

        void Add(Type stateType, IBlazorStateComponent blazorStateComponent);

        void Remove(IBlazorStateComponent blazorStateComponent);
    }
}
