using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apparatus.Blazor.State.Contracts
{
    /// <summary>
    /// Manages the current state and notifies subscribers when state changes.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    public interface IStore<TState> where TState : class, IState
    {
        /// <summary>
        /// Gets the current state.
        /// </summary>
        TState State { get; }

        /// <summary>
        /// Updates the state and notifies all subscribers.
        /// </summary>
        /// <param name="state">The new state.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetState(TState state);

        /// <summary>
        /// Triggers a refresh, notifying all subscribers without changing state.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task Refresh();
    }
}
