using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apparatus.Blazor.State.Contracts
{
    public interface IStore<TState> where TState : class, IState
    {
        TState State { get; }

        Task SetState(TState state);

        Task Refresh();
    }
}
