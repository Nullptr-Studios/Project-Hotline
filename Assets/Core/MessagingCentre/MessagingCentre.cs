using System;
using System.Collections.Generic;
using System.Reflection;

namespace CC.MessagingCentre
{
    public class MessagingCentre
    {
        private Dictionary<Publisher, List<Subscription>> _subscriptions = new Dictionary<Publisher, List<Subscription>>();
        private static MessagingCentre Instance { get; } = new MessagingCentre();

        #region Sends

        /// <summary>
        /// Publish the message to any subscribers
        /// <code>MessagingCentre.Publish&lt;TPublisher&gt;(publishingObject, message)</code>
        /// </summary>
        /// <typeparam name="TPublisher">Publisher Type</typeparam>
        /// <param name="publisher">Publisher object</param>
        /// <param name="message">Message to publish</param>
        public static void Publish<TPublisher>(TPublisher publisher, string message) where TPublisher : class
        {
            Instance.publish(typeof(TPublisher), publisher, message, null, null);
        }

        /// <summary>
        /// Publish the message to any subscribers with argument data
        /// <code>MessagingCentre.Publish&lt;TPublisher, TArgs&gt;(publishingObject, message, data)</code>
        /// </summary>
        /// <typeparam name="TPublisher">Publisher Type</typeparam>
        /// <typeparam name="TArgs">Argument Type</typeparam>
        /// <param name="publisher">Publisher object</param>
        /// <param name="message">Message to publish</param>
        /// <param name="args">Argument data</param>
        public static void Publish<TPublisher, TArgs>(TPublisher publisher, string message, TArgs args) where TPublisher : class
        {
            Instance.publish(typeof(TPublisher), publisher, message, typeof(TArgs), args);
        }

        // Do the actual publishing here
        private void publish(Type publisherType, object publisher, string message, Type argType, object args)
        {
            if (message == null)
                throw new ArgumentException(nameof(message));

            // Check if there's any subscribers to the message
            var key = new Publisher(message, publisherType, argType);
            if (!_subscriptions.ContainsKey(key))
                return;

            // The loop subs and invoke the callback method
            foreach (var sub in _subscriptions[key])
            {
                // Check that the subscriber hasn't unsubscribed or been collected since registering
                if (sub.Subscriber != null)
                    sub.InvokeCallback(publisher, args);
            }
        }

        #endregion

        #region Subscriptions

        /// <summary>
        /// Subscribes to a message
        /// <code>MessaginCentre.Subscribe&lt;TPublisher&gt;(subscribingObject, message, callbackDelegate)</code>
        /// </summary>
        /// <typeparam name="TPublisher">Publisher Type</typeparam>
        /// <param name="sub">Subscriber object</param>
        /// <param name="message">Message subscribing to</param>
        /// <param name="callback">Callback</param>
        public static void Subscribe<TPublisher>(object sub, string message, Action<TPublisher> callback) where TPublisher : class
        {
            Instance.subscribe(sub, callback.Target, message, typeof(TPublisher), null, callback.GetMethodInfo());
        }

        /// <summary>
        /// Subscribes to a message
        /// <code>MessaginCentre.Subscribe&lt;TPublisher, TArgs&gt;(subscribingObject, message, callbackDelegate)</code>
        /// </summary>
        /// <typeparam name="TPublisher">Publisher Type</typeparam>
        /// <typeparam name="TArgs">Argument Tyoe</typeparam>
        /// <param name="sub">Subscriber object</param>
        /// <param name="message">Message to subscribe to</param>
        /// <param name="callback">Callback</param>
        public static void Subscribe<TPublisher, TArgs>(object sub, string message, Action<TPublisher, TArgs> callback) where TPublisher : class
        {
            Instance.subscribe(sub, callback.Target, message, typeof(TPublisher), typeof(TArgs), callback.GetMethodInfo());
        }

        // Do the subscribing here
        private void subscribe(object sub, object target, string message, Type publisherType, Type argType, MethodInfo methodInfo)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var key = new Publisher(message, publisherType, argType);
            var value = new Subscription(sub, target, methodInfo);

            // Publisher exists, add the new subscription
            if (_subscriptions.ContainsKey(key))
                _subscriptions[key].Add(value);
            // Doesn't exist, create the subscription list
            else
                _subscriptions[key] = new List<Subscription> { value };
        }

        #endregion

        #region Unsubscribes

        /// <summary>
        /// Unsubscribe an object from a message
        /// <code>MessagingCentre.Unsubscribe&lt;TPublisher&gt;(subscriptionObject, message)</code>
        /// </summary>
        /// <typeparam name="TPublisher">Publisher Type</typeparam>
        /// <param name="sub">Subscription object</param>
        /// <param name="message">Message to unsubscribe from</param>
        public static void Unsubscribe<TPublisher>(object sub, string message) where TPublisher : class
        {
            Instance.unsubscribe(sub, message, typeof(TPublisher), null);
        }

        /// <summary>
        /// Unsubscribe an object from a message that includes arguments
        /// <code>MessagingCentre.Unsubscribe&lt;TPublisher, TArgs&gt;(subscriptionObject, message)</code>
        /// </summary>
        /// <typeparam name="TPublisher">Publisher Type</typeparam>
        /// <typeparam name="TArgs">Argument Type</typeparam>
        /// <param name="sub">Subscription object</param>
        /// <param name="message">Message to unsubscribe from</param>
        public static void Unsubscribe<TPublisher, TArgs>(object sub, string message) where TPublisher : class
        {
            Instance.unsubscribe(sub, message, typeof(TPublisher), typeof(TArgs));
        }

        // Do the unsubscribing here
        private void unsubscribe(object sub, string message, Type publisherType, Type argType)
        {
            if (sub == null)
                throw new ArgumentNullException(nameof(sub));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var key = new Publisher(message, publisherType, argType);

            // Not subscribed anyway
            if (!_subscriptions.ContainsKey(key))
                return;

            // Remove all subscriptions from this subscriber that match the publisher object
            _subscriptions[key].RemoveAll(s => s.Subscriber == sub);
        }

        #endregion
    }
}