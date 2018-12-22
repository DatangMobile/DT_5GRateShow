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
using System.IO;

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

        public static int Coef = 1;
        public static int CoefSave;
        public static int Cliff;

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

        // 如果文件不存在则创建文件如果存在则覆盖文件;
        FileStream fs;
        string ConfigNum;

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
            fs = new FileStream("log.txt", FileMode.Create, FileAccess.Write);
            this.Closed += MainWindow_Closed;
            
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //sw.Flush();
            //sw.Close();
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
                    Thread.Sleep(1000);
                }
            });
            tsk.Start();
            
            StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"\conf.txt", Encoding.Default);
            while ((ConfigNum = sr.ReadLine()) != null)
            {
                Console.WriteLine(ConfigNum.ToString());
                string PrintString = "";
                try
                {
                    if (ConfigNum.Substring(0, 4) == "优化倍数")
                    {
                        Coef = int.Parse(ConfigNum.Substring(4, ConfigNum.Length - 4));
                        CoefSave = Coef;
                        PrintString = "写入优化倍数:" + Coef + "\n";
                    }
                    else if (ConfigNum.Substring(0, 4) == "爬升系数")
                    {
                        Cliff = int.Parse(ConfigNum.Substring(4, ConfigNum.Length - 4));
                        PrintString = "写入爬升系数:" + Cliff + "\n";
                    }


                    try
                    {
                        fs.Close();
                        fs = File.OpenWrite(System.Environment.CurrentDirectory + @"\log.txt");
                        fs.Position = fs.Length;
                        byte[] bytes = Encoding.UTF8.GetBytes(PrintString);
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    catch (Exception e2)
                    {
                        Console.WriteLine("write file log.txt failed:" + e2.ToString());
                    }
                    finally
                    {
                        fs.Close();
                    }

                }
                catch
                {

                }

            }
        }

        private void GetNetWorkRate()
        {
            if(!GetRate)
            {
                return;
            }

            string now_Time = DateTime.Now.ToString();
            string dl_rate = String.Format("DownloadSpeedKbps {0:n} Mbps ", adapter.DownloadSpeedKbps / 1024 * 8);
            string ul_rate = String.Format("UploadSpeedKbps {0:n} Mbps", adapter.UploadSpeedKbps / 1024 * 8);
            string PrintString = now_Time + " : " + dl_rate + ul_rate + "\n";

            // 对数据进行优化处理;
            double dl = (adapter.DownloadSpeedKbps / 1024 * Coef * 8);
            double ul = (adapter.UploadSpeedKbps / 1024 * Coef * 8);

            if(CoefSave != 1)
            {
                // 表明有业务正在运行;
                if (dl > 2f)
                {
                    if (Coef < CoefSave)
                    {
                        Cliff += 2;
                        Coef += Cliff;
                        if(Coef > CoefSave)
                        {
                            Coef = CoefSave;
                        }
                    }
                    if (dl > 1700f)
                    {
                        dl = 1700f;
                        dl -= (new Random()).NextDouble();
                        dl -= (new Random()).Next(1, 5);
                    }
                    // 如果低于1.4G速率,可以显示出波段;
                    else if (dl < 1400f)
                    {
                        // 如果正在爬升中，则显示真实优化的速率;
                        if(Coef < CoefSave)
                        {
                        }
                        else
                        {
                            dl = 1400f;
                            dl += (new Random()).NextDouble();
                            dl += (new Random()).Next(1, 20);
                        }
                    }
                }
                // 表明没有业务;
                else if (dl < 2f)
                {
                    Coef = 1;

                }
            }
            else if(CoefSave == 1)
            {
                // 特殊处理,倍率是1的时候，显示真实数据;
            }
            
            
                
            // 向前端发送速率数据;
            m_TransmitData.SetRate(ul.ToString(), dl.ToString());


            // 写入log文件;
            try
            {
                fs = File.OpenWrite(System.Environment.CurrentDirectory + @"\log.txt");
                fs.Position = fs.Length;
                byte[] bytes = Encoding.UTF8.GetBytes(PrintString);
                fs.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("write file log.txt failed:" + e.ToString());
            }
            finally
            {
                fs.Close();
            }


            Console.WriteLine(PrintString);
        }


        private void clickdebug(object sender, RoutedEventArgs e)
        {
            this.Rate_Address.ShowDevTools();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Rate_Address.Height = e.NewSize.Height;
            this.Rate_Address.Width = e.NewSize.Width;
            try
            {
                this.Rate_Address.Reload();
            }
            catch
            {

            }
            
        }
    }
}
