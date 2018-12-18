using RateMainWindow.NetWorkMonitor;
using System.Windows;

namespace RateMainWindow
{
    /// <summary>
    /// SelectNetAdapters.xaml 的交互逻辑
    /// </summary>
    public partial class SelectNetAdapters : Window
    {
        /// <summary>
        /// 网络适配器列表;
        /// </summary>
        public NetworkAdapter[] adapters;

        /// <summary>
        /// 网速监控器;
        /// </summary>
        public NetworkMonitor monitor;

        public SelectNetAdapters()
        {
            InitializeComponent();

            monitor = new NetworkMonitor();
            adapters = monitor.Adapters;

            if (adapters.Length == 0)
            {
                MessageBox.Show("没有检测到网卡，请检查配置");
                return;
            }

            this.AdapterList.ItemsSource = adapters;
            //this.GetRateStatus.IsChecked = true;
        }

        /// <summary>
        /// 用户选择网卡;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserSelectedAdapter(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// 确认用户选择的网卡;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetAdapter(object sender, RoutedEventArgs e)
        {
            MainWindow.adapter = (NetworkAdapter)this.AdapterList.SelectedItem;
            this.Close();
        }
    }
}
