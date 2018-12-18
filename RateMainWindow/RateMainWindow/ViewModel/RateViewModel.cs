using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateMainWindow.Model;

namespace RateMainWindow
{
    public class RateViewModel
    {
        /// <summary>
        /// 向前端传递数据的模型;
        /// </summary>
        private string TransmitionData;
        public string m_TransmitionData
        {
            get
            {
                return TransmitionData;
            }
            set
            {
                TransmitionData = value;
            }
        }


        public RateViewModel()
        {
        }

        /// <summary>
        /// 初始化数据，并将数据转化为JSON格式;
        /// </summary>
        /// <returns></returns>
        private string FromDataToJson(RateJSModel data)
        {
            return DataConverter.ObjectToJson(data);
        }

        /// <summary>
        /// 设置上下行速率并传递给前端页面;
        /// </summary>
        /// <param name="up"></param>
        /// <param name="down"></param>
        public void SetRate(string up, string down)
        {
            RateJSModel data = new RateJSModel();
            data.UpLoadRate = up;
            data.DownLoadRate = down;
            m_TransmitionData = FromDataToJson(data);
        }
    }
}
