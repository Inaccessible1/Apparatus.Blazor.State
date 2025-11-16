using Apparatus.Blazor.State.Contracts;
using System.Collections;

namespace Apparatus.Blazor.State
{
    internal class ActionSubscriber : IActionSubscriber
    {
        private readonly Dictionary<Type, IList> _subscriptions;
        private readonly object _lock = new();

        public ActionSubscriber()
        {
            _subscriptions = new Dictionary<Type, IList>();
        }

        public void Publish<TAction>(TAction action) where TAction : IAction
        {
            var actionType = typeof(TAction);

            lock (_lock)
            {
                if (_subscriptions.TryGetValue(actionType, out IList? value))
                {
                    var actionSubList = new List<ActionSubscription<TAction>>(value.Cast<ActionSubscription<TAction>>());

                    foreach (var actionSub in actionSubList)
                    {
                        actionSub.Delegate?.Invoke(action);
                    }
                }
            }
        }

        public string Subscribe<TAction>(Action<TAction> @delegate) where TAction : IAction
        {
            if (@delegate == null)
            {
                throw new ArgumentNullException(nameof(@delegate));
            }

            var actionType = typeof(TAction);
            var id = $"{@delegate.Target?.GetType().FullName}_{@delegate.Target?.GetHashCode()}_{Guid.NewGuid():N}";

            var newAction = new ActionSubscription<TAction>(actionType, @delegate, id);

            lock (_lock)
            {
                if (!_subscriptions.TryGetValue(actionType, out IList? actionSubList))
                {
                    actionSubList = new List<ActionSubscription<TAction>>
                    {
                        newAction
                    };
                    _subscriptions.Add(actionType, actionSubList);
                }
                else
                {
                    if (actionSubList is List<ActionSubscription<TAction>> asl)
                    {
                        asl.Add(newAction);
                    }
                }
            }

            return id;
        }

        /// <summary>
        /// Unsubscribes a specific action subscription.
        /// </summary>
        /// <typeparam name="TAction">The action type.</typeparam>
        /// <param name="subscriptionId">The subscription identifier.</param>
        public void Unsubscribe<TAction>(string subscriptionId) where TAction : IAction
        {
            if (string.IsNullOrEmpty(subscriptionId))
            {
                throw new ArgumentException("Subscription ID cannot be null or empty", nameof(subscriptionId));
            }

            var actionType = typeof(TAction);

            lock (_lock)
            {
                if (_subscriptions.TryGetValue(actionType, out IList? actionSubList))
                {
                    if (actionSubList is List<ActionSubscription<TAction>> asl)
                    {
                        var subscription = asl.FirstOrDefault(s => s.Id == subscriptionId);
                        if (subscription.Id != null)
                        {
                            asl.Remove(subscription);

                            if (asl.Count == 0)
                            {
                                _subscriptions.Remove(actionType);
                            }
                        }
                    }
                }
            }
        }
    }
}
