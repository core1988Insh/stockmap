using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ChartBaseService
{
    /// <summary>
    /// 自定义颜色
    /// </summary>
    public class CustomLine
    {
        //线样式
        public string LineStyle { get; set; }
        //线颜色
        public Color LineColor { get; set; }
        //开始点
        public Point StartPoint { get; set; }
        //结束点
        public Point EndPoint { get; set; }
        //线粗细
        public int LineSize { get; set; }
        
    }

}
