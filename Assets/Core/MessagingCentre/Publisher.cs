using System;

namespace CC.MessagingCentre
{
    /// <summary>
    /// Object that represents a publisher, used as key in the subscriptions dictionary
    /// </summary>
    internal class Publisher
    {
        public string Message { get; private set; }
        public Type PublisherType { get; private set; }
        public Type ArgType { get; private set; }

        // Initialize
        public Publisher(string message, Type publisherType, Type argType)
        {
            Message = message;
            PublisherType = publisherType;
            ArgType = argType;
        }

        // Override so the object can be used a dictionary key
        public override int GetHashCode()
        {
            return Message.GetHashCode();
        }

        // Check to see if the publisher is the same
        public override bool Equals(object obj)
        {
            var toTest = obj as Publisher;

            return toTest != null &&
                toTest.Message == Message &&
                toTest.PublisherType == PublisherType &&
                toTest.ArgType == ArgType;
        }
    }
}