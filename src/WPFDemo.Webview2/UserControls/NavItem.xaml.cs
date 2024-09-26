using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFDemo.Webview2.UserControls
{
    /// <summary>
    /// NavItem.xaml 的交互逻辑
    /// </summary>
    public partial class NavItem : UserControl
    {
        public event RoutedEventHandler OnClick;
        public event RoutedEventHandler OnClose;
        public event EventHandler<string> OnTitleChanged;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); OnTitleChanged?.Invoke(this, Title); }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(NavItem), new PropertyMetadata("标签页"));
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(NavItem), new PropertyMetadata(false));


        public NavItem()
        {
            InitializeComponent();
        }

        private void Close(object sender, MouseButtonEventArgs e)
        {
            OnClose?.Invoke(this, e);
        }

        private void Click(object sender, MouseButtonEventArgs e)
        {
            OnClick?.Invoke(this, e);

        }
    }
}
