using Apparatus.Blazor.State.Contracts;

namespace Apparatus.Blazor.State
{
    internal class SubscriptionService : ISubscriptionService
    {
        private readonly List<StateSubscription> blazorStateComponentReferencesList;
        private readonly Lock _lock = new();

        public SubscriptionService()
        {
            blazorStateComponentReferencesList = [];
        }

        public void Add(Type stateType, IBlazorStateComponent blazorStateComponent)
        {
            lock (_lock)
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
        }

        public void Remove(IBlazorStateComponent blazorStateComponent)
        {
            lock (_lock)
            {
                blazorStateComponentReferencesList.RemoveAll(item => item.ComponentId == blazorStateComponent.Id);
            }
        }

        public void ReRenderSubscribers(Type stateType)
        {
            List<StateSubscription> subscriptions;
            
            lock (_lock)
            {
                subscriptions = [.. blazorStateComponentReferencesList.Where(record => record.StateType == stateType)];
            }

            var subscriptionsToRemove = new List<StateSubscription>();

            foreach (StateSubscription subscription in subscriptions)
            {
                if (subscription.BlazorStateComponentReference.TryGetTarget(out IBlazorStateComponent? component))
                {
                    component.ReRender();
                }
                else
                {
                    subscriptionsToRemove.Add(subscription);
                }
            }

            // Clean up dead references
            if (subscriptionsToRemove.Count > 0)
            {
                lock (_lock)
                {
                    foreach (var subscription in subscriptionsToRemove)
                    {
                        blazorStateComponentReferencesList.Remove(subscription);
                    }
                }
            }
        }

        public void ReRenderSubscribers<TState>() where TState : IState
        {
            ReRenderSubscribers(typeof(TState));
        }
    }
}
