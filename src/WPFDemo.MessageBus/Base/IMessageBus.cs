using WPFDemo.MessageBus.Dtos;

namespace WPFDemo.MessageBus.Base
{
    public interface IMessageBus
    {
        bool RegisterService(IBusService service);
        void RegisterChannel(IMessageChannel channel);
        void UnregisterChannel(int channelId);

        void SendServiceMessage(Message message, int channelId);
        void PublishEvent(Message message, string routingKey);
    }
}
