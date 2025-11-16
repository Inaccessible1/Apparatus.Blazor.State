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
                if (_subscriptions.ContainsKey(actionType))
                {
                    var actionSubList = new List<ActionSubscription<TAction>>(
                        _subscriptions[actionType].Cast<ActionSubscription<TAction>>());

                    foreach (var actionSub in actionSubList)
                    {
                        actionSub.Delegate?.Invoke(action);
                    }
                }
            }
        }

        public void Subscribe<TAction>(Action<TAction> @delegate) where TAction : IAction
        {
            var actionType = typeof(TAction);
            var id = $"{@delegate?.Target?.GetType().FullName}_{@delegate?.Target?.GetHashCode()}";

            var newAction = new ActionSubscription<TAction>(actionType, @delegate, id);

            lock (_lock)
            {
                if (!_subscriptions.TryGetValue(actionType, out IList? actionSubList))
                {
                    actionSubList = new List<ActionSubscription<TAction>>();
                    actionSubList.Add(newAction);
                    _subscriptions.Add(actionType, actionSubList);
                }
                else
                {
                    var asl = actionSubList as List<ActionSubscription<TAction>>;

                    if (asl != null && !asl.Any(li => li.Id == id))
                    {
                        asl.Add(newAction);
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes a specific action subscription.
        /// </summary>
        /// <typeparam name="TAction">The action type.</typeparam>
        /// <param name="subscriptionId">The subscription identifier.</param>
        public void Unsubscribe<TAction>(string subscriptionId) where TAction : IAction
        {
            var actionType = typeof(TAction);

            lock (_lock)
            {
                if (_subscriptions.TryGetValue(actionType, out IList? actionSubList))
                {
                    var asl = actionSubList as List<ActionSubscription<TAction>>;
                    if (asl != null)
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
