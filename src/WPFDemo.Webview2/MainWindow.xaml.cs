using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFDemo.Webview2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConfigOptions _options;

        public MainWindow()
        {
            _options = App.ServiceProvider.GetService<IOptions<ConfigOptions>>()!.Value;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            OpenWebView2(_options.Home);
        }

        private void OpenWebView2(string url)
        {
            var wv = new WebView2();

            wv.CoreWebView2InitializationCompleted += CoreWebView2_InitializationCompleted;
        }

        private void CoreWebView2_InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            var wv = (sender as WebView2)!;

            wv.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DiskCache);
        }

        #region Button Click
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnMinimized_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnMaximized_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}