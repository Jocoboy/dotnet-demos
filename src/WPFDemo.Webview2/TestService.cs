using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFDemo.MessageBus;
using WPFDemo.MessageBus.Dtos;

namespace WPFDemo.Webview2
{
    public class TestService : BusService
    {
        public override string ServiceName => "TestService";

        public void Handle(Message<TestData> message, MessageContext messageContext)
        {
            var replyData = new ReplyData();
            replyData.Data.Add(message.Data.TestName);
            var replyMsg = new Message<ReplyData>
            {
                ServiceName = ServiceName,
                CorrelationId = message.Id,
                DataType = "ReplyData",
                Data = replyData
            };
            bus.SendServiceMessage(replyMsg, messageContext.ChannelId);
        }
    }

    public class TestData
    {
        public string TestName { get; set; }
    }

    public class ReplyData
    {
        public List<string> Data { get; set; } = new();
    }
}
