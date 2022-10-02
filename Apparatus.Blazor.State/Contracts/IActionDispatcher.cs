using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Apparatus.Blazor.State.Contracts
{
    public interface IActionDispatcher
    {
        Task Dispatch<TAction>(TAction action) where TAction : IAction;

        Task Dispatch(IAction action);
    }
}
