using Apparatus.Blazor.State.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apparatus.Blazor.State
{
    internal readonly struct ActionSubscription<TAction> where TAction : IAction
    {
        public Action<TAction>? Delegate { get; }

        public string Id { get; }

        public Type ActionType { get; }

        public ActionSubscription(Type actionType, Action<TAction>? delegete, string id)
        {
            ActionType = actionType;
            Delegate = delegete;
            Id = id;
        }
    }
}
