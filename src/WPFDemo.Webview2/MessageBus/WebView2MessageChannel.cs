using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFDemo.MessageBus;

namespace WPFDemo.Webview2.MessageBus
{
    public class WebView2MessageChannel : MessageChannel
    {
        public WebView2 WebView { get; private set; }

        public WebView2MessageChannel(WebView2 webView)
        {
            WebView = webView;
            webView.WebMessageReceived += WebView_WebMessageReceived;
            webView.NavigationStarting += WebView_NavigationStarting;
        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            SynchronizationContext.Current.Post((_) =>
            {
                OnMessageReceived(e.WebMessageAsJson);
            }, null);
        }

        private void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            ClearSubscriptions();
        }

        protected override void SendMessage(string message)
        {
            // 检测是否在控件的UI线程上执行
            if (WebView.Dispatcher.CheckAccess())
            {
                try
                {
                    WebView.CoreWebView2?.PostWebMessageAsJson(message);
                }
                catch (ObjectDisposedException) { }
            }
            else
            {
                // 不在同一个线程就使用线程调度方式执行
                WebView.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        WebView.CoreWebView2?.PostWebMessageAsJson(message);
                    }
                    catch (ObjectDisposedException) { }
                });
            }
        }
    }
}
