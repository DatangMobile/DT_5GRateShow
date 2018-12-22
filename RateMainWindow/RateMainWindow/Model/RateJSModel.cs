using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMainWindow.Model
{
    /// <summary>
    /// 传递给前端JS的数据模型;
    /// </summary>
    class RateJSModel
    {
        public string DownLoadRate { get; set; }
        public string UpLoadRate { get; set; }

        public string DetailRateFontSize { get; set; }
        public string DetailRateOffset { get; set; }
    }
}
