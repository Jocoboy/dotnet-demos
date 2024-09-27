namespace WPFDemo.MessageBus.Dtos
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? CorrelationId { get; set; }
        public string ServiceName { get; set; }
        public string DataType { get; set; }
    }

    public class Message<T> : Message
    {
        public T Data { get; set; }

        public Message() { }
        public Message(T data)
        {
            Data = data;
            DataType = typeof(T).Name;
        }
    }
}
