using WPFDemo.MessageBus.Dtos;

namespace WPFDemo.MessageBus.Base
{
    public interface IBusService
    {
        string ServiceName { get; }

        void OnBusRegistry(IMessageBus messageBus);
        void DispatchMessage(MessageContext messageContext);
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
