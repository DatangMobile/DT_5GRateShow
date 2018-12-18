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
            
            this.Rate_Address.Address = System.Environment.CurrentDirectory + @"\ViewPage\pages\AboutLeafLet.html";
            CefSharp.CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            
            this.Rate_Address.BeginInit();
            
            // 启动监控线程;
            Task tsk = new Task(() =>
            {
                while (GetRate)
                {
                    GetNetWorkRate();
                    Thread.Sleep(1000);
                }
            });
            tsk.Start();
        }

        private void GetNetWorkRate()
        {
            string dl_rate = String.Format("DownloadSpeedKbps {0:n} kbps", adapter.DownloadSpeedKbps);
            string ul_rate = String.Format("UploadSpeedKbps {0:n} kbps", adapter.UploadSpeedKbps);
            
            this.Rate_Address.RegisterJsObject("JsObj", m_TransmitData);             // 向前端页面注册一个JsObj，前端可以通过这个进行交互;

            Console.WriteLine(dl_rate + ul_rate);
        }
    }
}
