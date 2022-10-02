using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apparatus.Blazor.State.Contracts
{
    public interface IActionHandler<in TAction> where TAction : IAction
    {
        Task Handle(TAction action);
    }
   
}
