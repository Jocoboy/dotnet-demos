using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBus = WPFDemo.MessageBus.MessageBus;

namespace WPFDemo.Webview2.MessageBus
{
    public class WebView2MessageBus : MsgBus
    {
        public void RegisterWebView(WebView2 webView)
        {
            var channel = new WebView2MessageChannel(webView);
            RegisterChannel(channel);
        }

        public void UnregisterWebView(WebView2 webView)
        {
            if (TryGetChannelId(webView, out var channelId))
            {
                UnregisterChannel(channelId);
            }
        }

        public void ActiveWebView(WebView2 webView)
        {
            if (TryGetChannelId(webView, out var channelId))
            {
                activeChannelId = channelId;
            }
        }

        #region private methods
        private bool TryGetChannelId(WebView2 webView, out int channelId)
        {
            channelId = -1;

            var pair = channels.SingleOrDefault(p => ((WebView2MessageChannel)p.Value).WebView == webView);

            if (pair.Value == null) return false;

            channelId = pair.Key;

            return true;
        }
        #endregion
    }
}
