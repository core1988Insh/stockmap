using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ChartBaseService
{
    /// <summary>
    /// 自定义行情图片属性类
    /// </summary>
    public struct CustomStockInfo
    {
        //图片高
        public int Height { get; set; }
        //图片宽
        public int Width { get; set; }
        //X轴移动值
        public int MoveX { get; set; }
        //Y轴移动值
        public int MoveY { get; set; }
        //字体大小
        public int FontSize { get; set; }
        //字体样式
        public string FontType { get; set; }
        //曲线宽度
        public int CurveThickness { get; set; }
        //线宽度
        public int LineThickness { get; set; }
        //字体颜色
        public Color FontColor { get; set; }
        //上升字体颜色
        public Color RiseFontColor { get; set; }
        //下降字体颜色
        public Color DeclineFontColor { get; set; }
        //曲线颜色
        public Color CurveColor { get; set; }
        //轴颜色
        public Color AxisColor { get; set; }
        //上边距
        public int TopMargin { get; set; }
        //下边距
        public int BottomMargin { get; set; }
        //左边距
        public int LeftMargin { get; set; }
        //右边距
        public int RightMargin { get; set; }
        //是否加粗
        public string Bold { get; set; }

    }
}
