using System.Reflection;

namespace CC.MessagingCentre
{
    /// <summary>
    /// Represents an objects subscription. Used to hold references to subscriber/target object, and the callback to be invoked when a message is published
    /// </summary>
    internal class Subscription
    {
        public object Subscriber { get; private set; }
        public object Target { get; private set; }
        public MethodInfo MethodInfo { get; private set; }

        // Initialize
        public Subscription(object subscriber, object target, MethodInfo methodInfo)
        {
            Subscriber = subscriber;
            Target = target;
            MethodInfo = methodInfo;
        }

        // Invoke the callback method and pass in any arguments if needed
        public void InvokeCallback(object publisher, object args)
        {
            // Check if we can call it straight away
            if (MethodInfo.IsStatic)
            {
                MethodInfo.Invoke(null, MethodInfo.GetParameters().Length == 1 ? new[] { publisher } : new[] { publisher, args });
                return;
            }

            // Don't invoke the callback if the subscriber has been collected
            if (Target == null)
                return;

            MethodInfo.Invoke(Target, MethodInfo.GetParameters().Length == 1 ? new[] { publisher } : new[] { publisher, args });
        }
    }
}