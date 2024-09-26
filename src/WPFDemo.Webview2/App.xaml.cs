using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WPFDemo.Webview2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Initialize();
        }

        private  void Initialize()
        {
            ServiceProvider = ConfigService();
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
