![workflow on wpfdemo-webview2 branch](https://github.com/Jocoboy/dotnet-demos/actions/workflows/dotnet-desktop.yml/badge.svg?branch=wpfdemo-webview2)

[:rewind:Back to Home](https://github.com/Jocoboy/dotnet-demos/tree/master)

## WPFDemo-Webview2

A demo for WPF Application using WebView2.

### Architecture

WebView2 Message Bus use "Publish/Subscribe" pattern. Each message will be delivered to multiple consumers (webview2 tabs) through different channels and will go to the queues whose binding key exactly matches the routing key of the message. And we use "CorrelationId" to mark which request the response belongs.

Here is the UML of WebView2 Message Bus in WPF.

![wpf_webview2_uml](https://jocoboy.github.io/Hexo-Blog/2024/08/13/abp-and-ddd/wpf_webview2_uml.png)

### Environment

- [C# 12](https://learn.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-12)
- [.NET 8.0](https://learn.microsoft.com/zh-cn/dotnet/core/whats-new/dotnet-8/overview) 
- [WPF .NET 8.0](https://learn.microsoft.com/zh-cn/dotnet/desktop/wpf/whats-new/net80) 

### Test

First, we create a service extends BusService.

```c#
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
```

Then, we subscribe the service, listen to the "message" webview event and send the message.

Finally, we will recieve the response with a service name and a correlationId.

```js
        var messagesElement = document.getElementById("messages");

        function SendSubscribeMessage() {
            try {
                var message = {
                    "Id": "2BE370A5-17AA-4747-8674-9E0E5FC0DF98",
                    "ServiceName": "Subscription",
                    "DataType": "Subscribe",
                    "Data": {
                        "ServiceName": "TestService",
                        "RoutingKeys": [
                            "123"
                        ]
                    }
                };
                window.chrome.webview.postMessage(message);

                var element = document.createElement('li');
                element.innerText = '发送订阅消息!';
                messagesElement.append(element);
            } catch(ex)
            {
                alert(`消息发送失败：${ex}`);
            }
        }

        function SendTestMessage() {
            try {
                var message = {
                    "Id": "2BE370A5-17AA-4747-8674-9E0E5FC0DF98",
                    "ServiceName": "TestService",
                    "DataType": "TestData",
                    "Data": {
                        "TestName": "Hello"
                    }
                };
                window.chrome.webview.postMessage(message);

                var element = document.createElement('li');
                element.innerText = '发送测试消息!';
                messagesElement.append(element);
            } catch (ex)
            {
                alert(`消息发送失败：${ex}`);
            }
        }

        window.onload = function () {
            window.chrome.webview.addEventListener('message', arg => {
                var element = document.createElement('li');
                element.innerText = `Host：${JSON.stringify(arg.data)}`;
                messagesElement.append(element);
                if (arg.data.ServiceName == "TestService") {
                    document.getElementById("replyData").innerText = '【消息已接收】';
                }
            });
        };
```

![wpf_webview2_test](https://jocoboy.github.io/Hexo-Blog/2024/08/13/abp-and-ddd/wpf_webview2_test.png)