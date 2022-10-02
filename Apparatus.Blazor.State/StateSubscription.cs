using Apparatus.Blazor.State.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apparatus.Blazor.State
{
    internal readonly struct StateSubscription
    {
        public WeakReference<IBlazorStateComponent> BlazorStateComponentReference { get; }
        public string ComponentId { get; }

        public Type StateType { get; }

        public StateSubscription(Type stateType, string componentId, WeakReference<IBlazorStateComponent> blazorStateComponentReference)
        {
            StateType = stateType;
            ComponentId = componentId;
            BlazorStateComponentReference = blazorStateComponentReference;
        }



    }
}
