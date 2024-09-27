using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using WPFDemo.MessageBus.Base;
using WPFDemo.MessageBus.Exceptions;
using WPFDemo.Webview2.MessageBus;

namespace WPFDemo.Webview2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static WebView2MessageBus MsgBus { get; } = new WebView2MessageBus();
        IBusService TestService;

        protected override void OnStartup(StartupEventArgs e)
        {
            Initialize();
            MsgBus.Error += MsgBus_Error;
        }

        private void MsgBus_Error(BusException obj)
        {
            MessageBox.Show($"消息总线服务在处理消息时出错:{obj.Message}");
        }

        private async void Initialize()
        {
            ServiceProvider = ConfigService();

            TestService = new TestService();
            await TestService.StartAsync();
            MsgBus.RegisterService(TestService);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await TestService.StopAsync();
        }

        private IServiceProvider ConfigService()
        {
            var configuration = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .AddEnvironmentVariables()
             .Build();

            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(configuration);

            services.Configure<ConfigOptions>(configuration.GetSection("App"));

            return services.BuildServiceProvider();
        }

    }

}
