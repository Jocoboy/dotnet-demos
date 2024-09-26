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
using System.Windows.Shell;
using WPFDemo.Webview2.UserControls;

namespace WPFDemo.Webview2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConfigOptions _options;

        #region control
        private Button BtnBack => Template.FindName("btnBack", this) as Button;
        private Button BtnForward => Template.FindName("btnForward", this) as Button;
        private StackPanel NavItems => Template.FindName("navs", this) as StackPanel;
        private StackPanel Titlebar => Template.FindName("titlebar", this) as StackPanel;
        private StackPanel Toolbar => Template.FindName("toolbar", this) as StackPanel;
        private StackPanel Features => Template.FindName("features", this) as StackPanel;
        #endregion

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

        private void OpenWebView2(string uri)
        {
            var wv = new WebView2();
            wv.CoreWebView2InitializationCompleted += CoreWebView2_InitializationCompleted;
            wv.EnsureCoreWebView2Async();
            wv.Source = new Uri(uri);
            //TODO: bus register
            tabs.Children.Add(wv);

            var nav = new NavItem();
            nav.SetValue(WindowChrome.IsHitTestVisibleInChromeProperty, true);
            nav.OnClick += Nav_Click;
            nav.OnClose += Nav_Close;
            nav.OnTitleChanged += (s, o) =>
            {
                NavAutoResize();
            };
            NavItems.Children.Add(nav);

            SelectWebView2(tabs.Children.Count - 1);
        }

        private void CloseWebView2(int index)
        {
            if (tabs.Children.Count <= 1) return;

            var selectedWv = GetSelectedWebView2();
            var selectedIndex = tabs.Children.IndexOf(selectedWv);

            var wv = tabs.Children[index] as WebView2;
            //TODO: bus unregister

            tabs.Children.RemoveAt(index);
            NavItems.Children.RemoveAt(index);
            NavAutoResize();

            if (selectedIndex == index)
            {
                selectedIndex = tabs.Children.Count == 1 ? 0 : (tabs.Children.Count <= selectedIndex ? selectedIndex - 1 : selectedIndex);
                SelectWebView2(selectedIndex);
            }

            wv!.Dispose();
        }

        private void CoreWebView2_InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            var wv = (sender as WebView2)!;
            var index = tabs.Children.IndexOf(wv);
            var nav = NavItems.Children[index] as NavItem;

            wv.CoreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.DiskCache);

            wv.CoreWebView2.DocumentTitleChanged += (o, e) =>
            {
                nav.Title = wv.CoreWebView2.DocumentTitle;
            };

            wv.SourceChanged += (o, e) =>
            {
                RefreshToolbarButtonState(wv.CoreWebView2);
            };

            wv.CoreWebView2.NewWindowRequested += (object sender, CoreWebView2NewWindowRequestedEventArgs e) =>
            {
                e.Handled = true;
                OpenWebView2(e.Uri);
            };

            wv.CoreWebView2.WindowCloseRequested += (object sender, object obj) =>
            {
                CloseWebView2(tabs.Children.IndexOf(wv));
            };
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var nav = sender as NavItem;
            var index = NavItems.Children.IndexOf(nav);
            SelectWebView2(index);
        }

        private void Nav_Close(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var nav = sender as NavItem;
            var index = NavItems.Children.IndexOf(nav);
            CloseWebView2(index);
        }

        #region private methods

        private void RefreshToolbarButtonState(CoreWebView2 coreWebView2)
        {
            BtnBack.IsEnabled = coreWebView2?.CanGoBack ?? false;
            BtnForward.IsEnabled = coreWebView2?.CanGoForward ?? false;
        }

        private WebView2 GetSelectedWebView2()
        {
            if (tabs.Children.Count == 0) return null;

            foreach (WebView2 item in tabs.Children)
            {
                if (item.Visibility == Visibility.Visible) return item;
            }

            return null;
        }

        private void SelectWebView2(int index)
        {
            if (tabs.Children.Count == 0) return;

            for (int i = 0; i < tabs.Children.Count; i++)
            {
                var item = tabs.Children[i] as WebView2;
                if (i == index)
                {
                    item.Visibility = Visibility.Visible;
                    //TODO: bus active
                    RefreshToolbarButtonState(item.CoreWebView2);
                }
                else
                {
                    item.Visibility = Visibility.Collapsed;
                    item.CoreWebView2?.TrySuspendAsync();
                }
            }

            for (int i = 0; i < NavItems.Children.Count; i++)
            {
                var nav = NavItems.Children[i] as NavItem;
                nav.IsActive = i == index;
            }
        }

        private void NavAutoResize()
        {
            if (NavItems.Children.Count == 0) return;
            var items = NavItems.Children.Cast<NavItem>().ToList();

            var availableWidth = Titlebar.RenderSize.Width - Toolbar.RenderSize.Width - Features.RenderSize.Width - NavItems.Margin.Left - NavItems.Margin.Right;
            var totalWidth = items.Sum(x => { x.Width = double.NaN; x.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity)); return x.DesiredSize.Width; });
            var avgWidth = availableWidth / NavItems.Children.Count;
            var notBeyondItems = items.Where(x => { x.Width = double.NaN; x.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity)); return availableWidth >= totalWidth || x.DesiredSize.Width <= avgWidth; }).ToList();
            var alterWidth = (availableWidth - notBeyondItems.Sum(x => x.DesiredSize.Width)) / (items.Count - notBeyondItems.Count);

            foreach (var item in items)
            {
                item.Width = !notBeyondItems.Contains(item) && alterWidth > 0 ? alterWidth : double.NaN;
            }
        }


        #endregion

        #region Button Click
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            if (tabs.Children.Count == 0)
            {
                OpenWebView2(_options.Home);
                return;
            }

            var wv = GetSelectedWebView2();
            wv.Source = new Uri(_options.Home);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (tabs.Children.Count == 0) return;

            var wv = GetSelectedWebView2();
            wv!.Reload();
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            var wv = GetSelectedWebView2();
            wv!.GoForward();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var wv = GetSelectedWebView2();
            wv!.GoBack();
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