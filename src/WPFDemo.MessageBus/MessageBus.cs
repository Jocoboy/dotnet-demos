using System.Collections.Concurrent;
using System.Text.Json;
using WPFDemo.MessageBus.Base;
using WPFDemo.MessageBus.Dtos;
using WPFDemo.MessageBus.Exceptions;

namespace WPFDemo.MessageBus
{
    public abstract class MessageBus : IMessageBus
    {
        private int currentMaxChannelId = 0;
        protected int activeChannelId = -1;
        public event Action<BusException> Error;

        protected readonly ConcurrentDictionary<int, IMessageChannel> channels = new();
        protected readonly ConcurrentDictionary<string, IBusService> services = new();

        /// <summary>
        /// 注册服务
        /// </summary>
        public bool RegisterService(IBusService service)
        {
            if (services.TryAdd(service.ServiceName, service))
            {
                service.OnBusRegistry(this);

                return true;
            }
            return false;
        }

        /// <summary>
        /// 创建消息通道
        /// </summary>
        public void RegisterChannel(IMessageChannel channel)
        {
            var channelId = Interlocked.Increment(ref currentMaxChannelId);
            channels.TryAdd(channelId, channel);

            channel.Id = channelId;
            channel.MessageReceived += ProcessClientMessage;
        }

        /// <summary>
        /// 关闭消息通道
        /// </summary>
        public void UnregisterChannel(int channelId)
        {
            if (channels.TryRemove(channelId, out var channel))
            {
                channel.MessageReceived -= ProcessClientMessage;
                channel.Id = -1;

                if (activeChannelId == channelId) activeChannelId = -1;
            }
        }

        /// <summary>
        /// 由消息总线找到激活的消息通道，发送消息
        /// </summary>
        public void SendServiceMessage(Message message, int channelId)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (channelId != activeChannelId) return;

            if (channels.TryGetValue(channelId, out var channel))
            {
                channel.SendMessage(message);
            }
        }

        /// <summary>
        /// 由消息总线找到激活的消息通道，发布订阅事件
        /// </summary>
        public void PublishEvent(Message message, string routingKey)
        {
            if (activeChannelId == -1) return;

            if (channels.TryGetValue(activeChannelId, out var channel))
            {
                channel.PublishEvent(routingKey, message);
            }
        }

        #region private methods

        private void ProcessSubscription(IMessageChannel channel, Message msg, string originalMessage)
        {
            switch (msg.DataType)
            {
                case "Subscribe":
                    var msg1 = JsonSerializer.Deserialize<Message<SubscriptionData>>(originalMessage);
                    channel.Subscribe(msg1.Data.ServiceName, msg1.Data.RoutingKeys);
                    break;
                case "Cancel":
                    var msg2 = JsonSerializer.Deserialize<Message<string>>(originalMessage);
                    channel.CancelSubscriptions(msg2.Data);
                    break;
                case "Clear":
                    channel.ClearSubscriptions();
                    break;
                default: throw new NotSupportedException($"未受支持的事件订阅操作:{msg.DataType}.");
            }
        }

        private void ProcessClientMessage(IMessageChannel channel, string message)
        {
            try
            {
                var msg = JsonSerializer.Deserialize<Message>(message);

                if (msg.ServiceName == "Subscription")
                {
                    ProcessSubscription(channel, msg, message);
                }
                else
                {
                    if (services.TryGetValue(msg.ServiceName, out var service))
                    {
                        var msgContext = new MessageContext(channel.Id, message, msg);

                        service.DispatchMessage(msgContext);
                    }
                    else throw new BusException($"Service [{msg.ServiceName}] is not found.");
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(new BusException(message, ex));
            }
        }
        #endregion
    }

    public class SubscriptionData
    {
        public string ServiceName { get; set; }
        public List<string> RoutingKeys { get; set; }
    }
}
