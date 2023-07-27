using System.Collections.Generic;
using System.Linq;
using System;

public class SubscriptionHolder : IDisposable
{
    private class SubscriptionInfo
    {
        public SubscriptionInfo(string key, object handler, IDisposable subscription)
        {
            Key = key;
            Handler = handler;
            Subscription = subscription;
        }

        public readonly string Key;
        public readonly object Handler;
        public readonly IDisposable Subscription;
    }

    private readonly EventService _eventService;
    private readonly List<SubscriptionInfo> _subscriptions = new List<SubscriptionInfo>();

    public SubscriptionHolder(EventService eventService)
    {
        _eventService = eventService;
    }

    public void Subscribe<TArgument>(Func<TArgument, bool> handler, string key = null)
    {
        _subscriptions.Add(new SubscriptionInfo(key, handler, _eventService.Subscribe(handler, key)));
    }

    public void Unsubscribe(string key)
    {
        var subs = _subscriptions.Where(s => s.Key != null && s.Key.Equals(key, StringComparison.Ordinal)).ToList();
        subs.ForEach(s =>
        {
            _subscriptions.Remove(s);
            s.Subscription.Dispose();
        });
    }

    public void Unsubscribe<TArgument>(Func<TArgument, bool> handler, string key = null)
    {
        var subs = _subscriptions.Where(s =>
            s.Handler as Func<TArgument, bool> == handler &&
            ((key == null && s.Key == null)) || (key != null && key.Equals(s.Key, StringComparison.Ordinal))).ToList();
        subs.ForEach(s =>
        {
            _subscriptions.Remove(s);
            s.Subscription.Dispose();
        });
    }

    public void Dispose()
    {
        _subscriptions.ForEach(s => s.Subscription.Dispose());
    }
}
