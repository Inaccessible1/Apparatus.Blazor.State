using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Apparatus.Blazor.State.Contracts
{
    /// <summary>
    /// Dispatches actions to their corresponding handlers and publishes them to subscribers.
    /// </summary>
    public interface IActionDispatcher
    {
        /// <summary>
        /// Dispatches a strongly-typed action.
        /// </summary>
        /// <typeparam name="TAction">The action type.</typeparam>
        /// <param name="action">The action to dispatch.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Dispatch<TAction>(TAction action) where TAction : IAction;

        /// <summary>
        /// Dispatches a weakly-typed action.
        /// </summary>
        /// <param name="action">The action to dispatch.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Dispatch(IAction action);
    }
}
