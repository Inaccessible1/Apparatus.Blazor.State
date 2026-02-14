using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apparatus.Blazor.State.Contracts
{
    /// <summary>
    /// Handles a specific action and updates the application state.
    /// </summary>
    /// <typeparam name="TAction">The action type this handler processes.</typeparam>
    public interface IActionHandler<in TAction> where TAction : IAction
    {
        /// <summary>
        /// Handles the specified action.
        /// </summary>
        /// <param name="action">The action to handle.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Handle(TAction action);
    }
}
