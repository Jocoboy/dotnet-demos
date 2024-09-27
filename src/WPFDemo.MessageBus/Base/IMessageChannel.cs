using WPFDemo.MessageBus.Dtos;

namespace WPFDemo.MessageBus.Base
{
    public interface IMessageChannel
    {
        int Id { get; set; }
        event Action<IMessageChannel, string> MessageReceived;

        void Subscribe(string serviceName, List<string> routingKeys);
        void CancelSubscriptions(string serviceName);
        void ClearSubscriptions();

        void SendMessage(Message message);
        void PublishEvent(string routingKey, Message message);
    }
}
