using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apparatus.Blazor.State.Contracts
{
    public interface IBlazorStateComponent
    {
        string Id { get; }

        TState GetState<TState>() where TState : IState, new();

        void ReRender();

    }
}
