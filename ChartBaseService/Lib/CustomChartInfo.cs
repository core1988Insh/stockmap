using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartBaseService
{
    public struct CustomChartInfo
    {
        //高比例尺
        public int HeightPercent { get; set; }
        //宽比例尺
        public int WidthPercent { get; set; }
        //背景颜色
        public Color BackColor { get; set; }
        //字体大小
        public int FontSize { get; set; }
        //字体
        public string FontType { get; set; }
        //字体颜色
        public Color FontColor { get; set; } 
        //X轴颜色
        public Color AxisXLineColor { get; set; }
        //Y轴颜色
        public Color AxisYLineColor { get; set; }
        //X轴宽度
        public int AxisXLineWidth { get; set; }
        //Y轴宽度
        public int AxisYLineWidth { get; set; }
        //X轴栅格线颜色
        public Color AxisXGridLineColor { get; set; }
        //Y轴栅格线颜色
        public Color AxisYGridLineColor { get; set; }
        //X轴栅格线宽度
        public int AxisXGridLineWidth { get; set; }
        //Y轴栅格线宽度
        public int AxisYGridLineWidth { get; set; }
        //X轴栅格线样式
        public string AxisXGridLineStyle { get; set; }
        //Y轴栅格线样式
        public string AxisYGridLineStyle { get; set; }
        //行情曲线颜色
        public Color StockLineColor { get; set; }
        //行情曲线宽度
        public int StockLineWidth { get; set; }
        //X轴值间距
        public int AxisXInterval { get; set; }
        //Y轴值间距
        public int AxisYInterval { get; set; }
        //Y轴最小值
        public double AxisYMinimum { get; set; }
        //Y轴最大值
        public double AxisYMaximum { get; set; } 
        //曲线样式
        public string LineStyle { get; set; }
        //保留小数位
        public int Digit { get; set; }
    }
}
