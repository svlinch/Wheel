using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EventService
{
    private class Subscription : IDisposable
    {
        public delegate bool MessageHandler(object message);

        private readonly EventService _service;
        private readonly Type _type;
        private readonly string _key;
        private readonly MessageHandler _handlerWrapper;

        public Subscription(EventService service, Type type, string key, MessageHandler handlerWrapper)
        {
            _service = service;
            _type = type;
            _key = key;
            _handlerWrapper = handlerWrapper;
        }

        public bool Handle(object obj)
        {
            return _handlerWrapper(obj);
        }

        public void Dispose()
        {
            _service._dictionary[_key][_type].Remove(this);
        }
    }

    private readonly Dictionary<string, Dictionary<Type, List<Subscription>>> _dictionary =
        new Dictionary<string, Dictionary<Type, List<Subscription>>>();

    public IDisposable Subscribe<TArgument>(Func<TArgument, bool> handler, string key = null)
    {
        if (key == null)
        {
            key = String.Empty;
        }
        var subscriptionType = typeof(TArgument);
        Dictionary<Type, List<Subscription>> subscriptionsByKey;
        if (!_dictionary.TryGetValue(key, out subscriptionsByKey))
        {
            subscriptionsByKey = new Dictionary<Type, List<Subscription>>();
            _dictionary[key] = subscriptionsByKey;
        }

        var subscription = new Subscription(this, subscriptionType, key, obj => handler((TArgument)obj));

        List<Subscription> subs;
        if (!subscriptionsByKey.TryGetValue(subscriptionType, out subs))
        {
            subs = new List<Subscription>();
            subscriptionsByKey[subscriptionType] = subs;
        }
        subs.Add(subscription);

        return subscription;
    }

    public void SendMessage<TArgument>(TArgument message, string key = null)
    {
        if (key == null)
        {
            key = String.Empty;
        }

        Dictionary<Type, List<Subscription>> subscriptionsByKey;
        if (!_dictionary.TryGetValue(key, out subscriptionsByKey))
        {
            Debug.Log(string.Format("No subscribers to key '{0}'", key));
            return;
        }

        List<Subscription> subs;
        if (!subscriptionsByKey.TryGetValue(typeof(TArgument), out subs))
        {
            Debug.Log(string.Format("No subscribers to key '{0}' with type {1}", key, typeof(TArgument)));
            return;
        }

        var subsCopy = subs.ToList();
        foreach (var subscription in subsCopy)
        {
            if (!subscription.Handle(message))
            {
                return;
            }
        }
    }
}
