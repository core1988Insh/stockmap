using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ChartBaseService
{
    /// <summary>
    /// 自定义文本
    /// </summary>
    public class CustomLabel
    {
        //文字
        public string Text { get; set; }
        //X轴坐标
        public int PositionX { get; set; }
        //Y轴坐标
        public int PositionY { get; set; }
        //字体
        public string FontType { get; set; }
        //字体大小
        public int FontSize { get; set; }      
        //字体颜色   
        public Color FontColor { get; set; }
        //是否粗体
        public bool Bold { get; set; }
    }

}
