using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartBaseService
{
    public class TaskInfo
    {
        //序号
        public int Id { get; set; }
        //股票代码
        public string StockCode { get; set; }
        //状态       
        private string status;
        private object locker = new object();
        public string Status
        {
            get { return status; }
            set
            {
                lock (locker)
                {
                    status = value;                 
                }

            }
        }
        //开始时间
        public string StartTime { get; set; }
        //结束时间
        public string FinishTime { get; set; }
        //文件保存路径
        public string FilePath { get; set; }
        //日志路径
        public string LogPath { get; set; }
        //名称
        public string Name { get; set; }
        //是否开启报警
        public bool Alert { get; set; }
        //昨收值
        public double LastClose { get; set; }
        //是否交易日
        public bool TradeDate { get; set; }
        //文件保存名
        public string SaveName { get; set; }
        //文件保存格式
        public string SaveType { get; set; }
        //项目
        public string Project { get; set; }
        //市场
        public string Market { get; set; }
        //宽度
        public int Width { get; set; }
        //高度
        public int Height { get; set; }
        //开盘时间
        public int[] TimeList { get; set; }
        //GDI+画分时图
        public CustomStockInfo customStockInfo;
        //Chart控件画K线图
        public CustomChartInfo customChartInfo;
        //添加文字信息
        public List<CustomLabel> customLabels = new List<CustomLabel>();
        //添加线段信息
        public List<CustomLine> customLines = new List<CustomLine>();
        //报警次数
        public int AlertTimes { get; set; }
        //报警时间
        public DateTime AlertTime { get; set; }
        //是否运行
        public bool RunFlag { get; set; }
        //X轴时间区间
        public double TimeSpan { get; set; }
        //添加文字
        public void AddLabels()
        {
            CustomLabel customLabel = new CustomLabel();

            customLabels.Add(customLabel);

        }
        //添加线段
        public void AddLines()
        {
            CustomLine customLine = new CustomLine();

            customLines.Add(customLine);

        }


    }


}
