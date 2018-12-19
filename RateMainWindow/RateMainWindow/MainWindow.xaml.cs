using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using RateMainWindow.NetWorkMonitor;
using CefSharp;

namespace RateMainWindow
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 网络适配器列表;
        /// </summary>
        private static NetworkAdapter m_adapter;
        public static NetworkAdapter adapter
        {
            get
            {
                return m_adapter;
            }
            set
            {
                m_adapter = value;
                monitor = new NetworkMonitor();
                monitor.StartMonitoring(m_adapter);
                GetRate = true;
                
            }
        }

        /// <summary>
        /// 是否获取网速;
        /// </summary>
        private static bool GetRate;

        public static int Coef = 100;

        /// <summary>
        /// 网速监控器;
        /// </summary>
        private static NetworkMonitor m_monitor;
        public static NetworkMonitor monitor
        {
            get
            {
                return m_monitor;
            }
            set
            {
                m_monitor = value;
            }
        }

        private RateViewModel m_TransmitData;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            GetRate = false;
            m_TransmitData = new RateViewModel();
            
            this.Rate_Address.Address = System.Environment.CurrentDirectory + @"\Pages\5GRateShow_UniqueDashboard\index2.html";
            CefSharp.CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            this.Rate_Address.RegisterJsObject("JsObj", m_TransmitData);             // 向前端页面注册一个JsObj，前端可以通过这个进行交互;
            this.Rate_Address.BeginInit();
        }

        /// <summary>
        /// 加载后显示网卡选择页面;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SelectNetAdapters selectNetAdaptersWindow = new SelectNetAdapters();
            selectNetAdaptersWindow.Show();
            

            
            // 启动监控线程;
            Task tsk = new Task(() =>
            {
                while (true)
                {
                    GetNetWorkRate();
                    Thread.Sleep(2000);
                }
            });
            tsk.Start();
        }

        private void GetNetWorkRate()
        {
            if(!GetRate)
            {
                return;
            }
            string dl_rate = String.Format("DownloadSpeedKbps {0:n} kbps ", adapter.DownloadSpeedKbps);
            string ul_rate = String.Format("UploadSpeedKbps {0:n} kbps", adapter.UploadSpeedKbps);

            double dl = (adapter.DownloadSpeedKbps / 1000 * Coef);
            double ul = (adapter.UploadSpeedKbps / 1000 * Coef);

            if (dl > 1700f)
                dl = 1700f;
            else if (dl < 1400)
                dl = 1400f;

            m_TransmitData.SetRate(ul.ToString(), dl.ToString());
            

            Console.WriteLine(dl_rate + ul_rate);
        }

        private void clickdebug(object sender, RoutedEventArgs e)
        {
            this.Rate_Address.ShowDevTools();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Rate_Address.Height = e.NewSize.Height;
            this.Rate_Address.Width = e.NewSize.Width;
        }
    }
}
