using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.State
{
    internal class SubscriptionService : ISubscriptionService
    {
        private readonly List<StateSubscription> blazorStateComponentReferencesList;

        public SubscriptionService()
        {
            blazorStateComponentReferencesList = new List<StateSubscription>();
        }

        public void Add(Type stateType, IBlazorStateComponent blazorStateComponent)
        {
            if (!blazorStateComponentReferencesList.Any(subscription => subscription.StateType == stateType && subscription.ComponentId == blazorStateComponent.Id))
            {
                var subscription = new StateSubscription(
                  stateType,
                  blazorStateComponent.Id,
                  new WeakReference<IBlazorStateComponent>(blazorStateComponent));

                blazorStateComponentReferencesList.Add(subscription);
            }
        }

        public void Remove(IBlazorStateComponent blazorStateComponent)
        {
            blazorStateComponentReferencesList.RemoveAll(item => item.ComponentId == blazorStateComponent.Id);
        }

        public void ReRenderSubscribers(Type stateType)
        {
            var subscriptions = blazorStateComponentReferencesList
                .Where(record => record.StateType == stateType)
                .ToList();

            foreach (StateSubscription subscription in subscriptions)
            {
                if (subscription.BlazorStateComponentReference.TryGetTarget(out IBlazorStateComponent? component))
                {
                    component.ReRender();
                }
                else
                {
                    blazorStateComponentReferencesList.Remove(subscription);
                }
            }
        }

        public void ReRenderSubscribers<TState>() where TState : IState
        {
            ReRenderSubscribers(typeof(TState));
        }
    }
}
