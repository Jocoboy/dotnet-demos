using System.Reflection;
using System.Text.Json;
using WPFDemo.MessageBus.Base;
using WPFDemo.MessageBus.Dtos;
using WPFDemo.MessageBus.Exceptions;

namespace WPFDemo.MessageBus
{
    public abstract class BusService : IBusService
    {
        protected IMessageBus bus;
        readonly Dictionary<string, (MethodInfo method, Type type)> handlers = new();

        public abstract string ServiceName { get; }

        protected BusService()
        {
            foreach (var method in GetType().GetMethods())
            {
                if (method.Name.StartsWith("Handle"))
                {
                    var type = method.GetParameters()[0].ParameterType;
                    handlers.Add(type.GenericTypeArguments[0].Name, (method, type));
                }
            }
        }

        public void OnBusRegistry(IMessageBus messageBus)
        {
            bus = messageBus;
        }

        public void DispatchMessage(MessageContext messageContext)
        {
            if (handlers.TryGetValue(messageContext.Message.DataType, out var handler))
            {
                var message = JsonSerializer.Deserialize(messageContext.OriginalMessage, handler.type);

                handler.method.Invoke(this, [message, messageContext]);
            }
            else throw new BusException($"The service method for data type [{messageContext.Message.DataType}] is not found in service [{ServiceName}].");
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
