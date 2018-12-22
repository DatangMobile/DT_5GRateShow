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
        /// <summary>
        /// 下载速率;
        /// </summary>
        public string DownLoadRate { get; set; }

        /// <summary>
        /// 上传速率;
        /// </summary>
        public string UpLoadRate { get; set; }

        /// <summary>
        /// 显示速率字符大小;
        /// </summary>
        public string DetailRateFontSize { get; set; }
        
        /// <summary>
        /// 显示速率字符的偏移量;
        /// </summary>
        public string DetailRateOffset { get; set; }
    }
}
