using System.Runtime.Serialization;

namespace WPFDemo.MessageBus.Exceptions
{
    public class BusException : Exception, ISerializable
    {
        public BusException() { }

        public BusException(string message) : base(message) { }

        public BusException(string message, Exception innerException) : base(message, innerException) { }
    }
}
