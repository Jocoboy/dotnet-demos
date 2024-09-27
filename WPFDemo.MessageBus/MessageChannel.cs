using System.Text.Json;
using WPFDemo.MessageBus.Base;
using WPFDemo.MessageBus.Dtos;

namespace WPFDemo.MessageBus
{
    public abstract class MessageChannel : IMessageChannel
    {
        public int Id { get ; set ; }

        public event Action<IMessageChannel, string> MessageReceived;

        readonly HashSet<(string ServiceName, string RoutingKey)> Subscriptions = new ();

        /// <summary>
        /// 订阅服务
        /// </summary>
        public void Subscribe(string serviceName, List<string> routingKeys)
        {
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));

            routingKeys?.ForEach(key =>
            {
                if (!string.IsNullOrEmpty(key))
                {
                    Subscriptions.Add((serviceName, key));
                }
            });
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public void CancelSubscriptions(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName)) return;

            Subscriptions.RemoveWhere(item => item.ServiceName.Equals(serviceName));
        }

        /// <summary>
        /// 清空订阅
        /// </summary>
        public void ClearSubscriptions() => Subscriptions.Clear();

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMessage(Message message)
        {
            var msg = JsonSerializer.Serialize(message, message.GetType());

            SendMessage(msg);
        }

        /// <summary>
        /// 发布订阅事件
        /// </summary>
        public void PublishEvent(string routingKey, Message message)
        {
            if (Subscriptions.Contains((message.ServiceName, routingKey)))
            {
                SendMessage(message);
            }
        }

        protected void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }

        protected abstract void SendMessage(string message);
    }
}
