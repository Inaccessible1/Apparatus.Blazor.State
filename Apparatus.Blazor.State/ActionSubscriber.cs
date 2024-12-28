using Apparatus.Blazor.State.Contracts;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Apparatus.Blazor.State
{
    internal class ActionSubscriber : IActionSubscriber
    {
        private Dictionary<Type, IList> subscriptions;

        public ActionSubscriber()
        {
            subscriptions = new Dictionary<Type, IList>();
        }

        public void Publish<TAction>(TAction action) where TAction : IAction
        {
            var actionType = typeof(TAction);

            IList actionSubList;
            if (subscriptions.ContainsKey(actionType))
            {
                actionSubList = new List<ActionSubscription<TAction>>(subscriptions[actionType].Cast<ActionSubscription<TAction>>());

                foreach (ActionSubscription<TAction> actionSub in actionSubList)
                {
                    if (actionSub.Delegate != null)
                        actionSub.Delegate(action);
                }
            }
        }

        public void Subscribe<TAction>(Action<TAction> delegete) where TAction : IAction
        {
            var actionType = typeof(TAction);
            var id = $"{delegete?.Target?.GetType().FullName}_{delegete?.Target?.GetHashCode()}";

            var newAction = new ActionSubscription<TAction>(actionType, delegete, id);

            IList? actionSubList;
            if (!subscriptions.TryGetValue(actionType, out actionSubList))
            {
                actionSubList = new List<ActionSubscription<TAction>>();
                actionSubList.Add(newAction);
                subscriptions.Add(actionType, actionSubList);
            }
            else
            {
                var asl = actionSubList as List<ActionSubscription<TAction>>;

                if (asl != null && !asl.Any(li => li.Id == id))
                    asl.Add(newAction);
            }
        }

        //public void UnSbscribe<TMessageType>(Subscription<TMessageType> subscriptions)
        //{
        //    Type t = typeof(TMessageType);
        //    if (subscriber.ContainsKey(t))
        //    {
        //        subscriber[t].Remove(subscription);
        //    }
        //}
    }
}
