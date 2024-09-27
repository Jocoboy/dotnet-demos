namespace WPFDemo.MessageBus.Dtos
{
    public class MessageContext
    {
        public Message Message { get; private set; }
        public string OriginalMessage { get; private set; }
        public int ChannelId { get; private set; }
        public DateTime ReceivedTime { get; private set; }

        public MessageContext(int channelId, string originalMessage, Message message)
        {
            Message = message;
            ChannelId = channelId;
            ReceivedTime = DateTime.Now;
            OriginalMessage = originalMessage;
        }
    }
}
