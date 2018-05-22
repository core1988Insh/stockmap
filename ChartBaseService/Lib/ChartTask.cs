using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartBaseService
{
    class ChartTask
    {
        TaskInfo stockInfo;
        ChartData data;
        string stockCode;
        string filePath;
        string logPath;
        int stockId;
        int width;
        int height;
        int moveX;
        int moveY;
        int curveSize;
        int lineSize;
        int fontSize;
        int top;
        int bottom;
        int left;
        int right;
        Color curveColor;
        Color axisColor;
        Color fontColor;
        string fontType;
        string name;
        double lastClose;  
        bool tradeDateFlag;
        string saveName;
        string saveType;
        FontStyle fontStyle;
        List<CustomLine> lines;
        List<CustomLabel> labels;

        public  void InitiTask(TaskInfo stockInfo)
        {
            this.stockInfo = stockInfo;           
            stockId = stockInfo.Id;
            stockCode = stockInfo.StockCode;
            filePath = stockInfo.FilePath;
            logPath = stockInfo.LogPath;
            name = stockInfo.Name;
            height = stockInfo.Height;
            width = stockInfo.Width;
          
            //股票自定义属性
            moveX = stockInfo.customStockInfo.MoveX;
            moveY = stockInfo.customStockInfo.MoveY;
            curveSize = stockInfo.customStockInfo.CurveThickness;
            lineSize = stockInfo.customStockInfo.LineThickness;
            fontSize = stockInfo.customStockInfo.FontSize;
            top = stockInfo.customStockInfo.TopMargin;
            bottom = stockInfo.customStockInfo.BottomMargin;
            left = stockInfo.customStockInfo.LeftMargin;
            right = stockInfo.customStockInfo.RightMargin;
            curveColor = stockInfo.customStockInfo.CurveColor;
            axisColor = stockInfo.customStockInfo.AxisColor;
            fontColor = stockInfo.customStockInfo.FontColor;
            fontType = stockInfo.customStockInfo.FontType;
            if (stockInfo.customStockInfo.Bold != "false")
            {
                fontStyle = FontStyle.Bold;
            }
            else
            {
                fontStyle = FontStyle.Regular;
            }
            saveName = stockInfo.SaveName;
            saveType = stockInfo.SaveType;
            lastClose = stockInfo.LastClose;
            tradeDateFlag = stockInfo.TradeDate;
            //自定义Label属性
            labels = stockInfo.customLabels;
            lines = stockInfo.customLines;
        }
        public Image Run()
        {
            InitiTask(TaskCache.taskInfo);
            data = new ChartData();
            data.InitiData(stockInfo);
            stockInfo.StartTime = DateTime.Now.ToString("HH:mm:ss fff");
            stockInfo.Status = "正在生成";

            //添加数据
            try
            {
                data.AddData();
            }
            catch (Exception ex)
            {
                stockInfo.Status = "获取数据失败";
                Manager.SyncWriteLog("Produce", logPath, "股票" + stockCode + "加载数据异常！详细信息："+ex.ToString());
                return null;
            }
            
            //绘图           
            Image resultImg;
            try
            {
                resultImg = DrawChart();
            }
            catch (Exception)
            {
                resultImg = null;
            }

            //异常不生产图片
            if (resultImg == null)
            {
                Manager.SyncWriteLog("Produce", logPath, "股票" + stockCode + "图片" + DateTime.Now.ToString("HH:mm:ss") + "生成失败！");
                Manager.ErrorAlert(stockInfo, stockInfo.Alert, "绘图失败", "股票" + stockCode + "图片" + DateTime.Now.ToString("HHmmss") + "生成失败！", logPath);
                stockInfo.Status = "出现错误";
                return null;
            }

            //保存图片
            Manager.SyncWriteLog("Produce", logPath, "股票" + stockCode + "图片" + DateTime.Now.ToString("HH:mm:ss") + "生成成功！");
            stockInfo.FinishTime = DateTime.Now.ToString("HH:mm:ss fff");
            stockInfo.Status = "已经生成";
            return resultImg;
        }
        //绘制图表
        public Image DrawChart()
        {
            int i = moveX;
            int j = moveY;
            double max = data.max;
            double min = data.min;
            double lastClose = data.lastClose;
            List<double> numbs = data.numbs;
            List<double> yValues = data.values;
            //昨收价不存在
            if (lastClose == 0)
            {
                return null;
            }
            //绘图
            Image img = new Bitmap(left + width + right, top + height + bottom);
            Graphics g = Graphics.FromImage(img);

            //绘图图片颜色
            SolidBrush brush = new SolidBrush(Color.White);
            Rectangle rect = new Rectangle(0, 0, left + right + width, top + bottom + height);
            g.FillRectangle(brush, rect);

            //添加XY轴栅格线
            Pen grayPen = new Pen(Color.LightGray, 1);
            g.DrawLine(grayPen, new Point(left + i, top + j), new Point(left + i, top + j + height));
            g.DrawLine(grayPen, new Point(left + i, top + j), new Point(left + i + width, top + j));
            g.DrawLine(grayPen, new Point(left + i + width, top + j), new Point(left + i + width, top + j + height));
            g.DrawLine(grayPen, new Point(left + i, top + j + height), new Point(left + i + width, top + j + height));

            g.DrawLine(grayPen, new Point(left + i + width / 2, top + j), new Point(left + i + width / 2, top + j + height));
            g.DrawLine(grayPen, new Point(left + i + width / 4, top + j), new Point(left + i + width / 4, top + j + height));
            g.DrawLine(grayPen, new Point(left + i + width * 3 / 4, top + j), new Point(left + i + width * 3 / 4, top + j + height));

            g.DrawLine(grayPen, new Point(left + i, top + j + height / 2), new Point(left + i + width, top + j + height / 2));
            g.DrawLine(grayPen, new Point(left + i, top + j + height / 4), new Point(left + i + width, top + j + height / 4));
            g.DrawLine(grayPen, new Point(left + i, top + j + height * 3 / 4), new Point(left + i + width, top + j + height * 3 / 4));

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //添加lable
            foreach (var item in labels)
            {
                AddLabels(item, g);
            }
            StringFormat strFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
            Font font = new Font(fontType, fontSize, FontStyle.Regular);
            g.DrawString(yValues[0].ToString("#0"), font, new SolidBrush(Color.Red), new Point(left + i, top + j - 2), strFormat);
            g.DrawString(yValues[1].ToString("#0"), font, new SolidBrush(Color.Red), new Point(left + i, top + j + height / 4 - 3), strFormat);
            g.DrawString(yValues[2].ToString("#0"), font, new SolidBrush(Color.Black), new Point(left + i, top + j + height / 2 - 4), strFormat);
            g.DrawString(yValues[3].ToString("#0"), font, new SolidBrush(Color.CornflowerBlue), new Point(left + i, top + j + height * 3 / 4 - 5), strFormat);
            g.DrawString(yValues[4].ToString("#0"), font, new SolidBrush(Color.CornflowerBlue), new Point(left + i, top + j + height - 6), strFormat);

            //添加曲线的数据
            List<PointF> points = new List<PointF>();
            if (numbs==null)
            {
                points.Add(new Point(left + i, top + j + height / 2 - 4));
            }
            else
            {
                for (int K = 0; K < numbs.Count; K++)
                {
                    PointF p = new PointF();
                    p.X = (float)(left + i + K * width/240);
                    p.Y = (float)(top + height + j - (height * (numbs[K] - min) / (max - min)));
                    points.Add(p);
                }
            }
            try
            {
                if (points.Count == 1)
                {
                    points.Add(points[0]);
                }
                g.DrawCurve(new Pen(curveColor, curveSize), points.ToArray());//画曲线
            }
            catch
            {
                Manager.SyncWriteLog("Produce", logPath, "绘画异常！ " + stockCode + "绘图失败！");
                StringBuilder sb = new StringBuilder();
                foreach (var item in numbs)
                {
                    sb.Append(item + "; ");
                }            
                return null;
            }
            return img;
        }

        protected  void AddText(string text, int fontSize, Color color, Point p, Graphics g)
        {

            SolidBrush brush = new SolidBrush(color);

            Font font = new Font(fontType, fontSize, fontStyle);

            StringFormat strFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
            g.DrawString(text, font, brush, p, strFormat);
        }

        //添加Y轴值
        protected  void AddValue(string text, CustomLabel label, Graphics g)
        {
            int lableX = label.PositionX;
            int lableY = label.PositionY;
            string lableFont = label.FontType;
            int labelSize = label.FontSize;
            Color lableColor = label.FontColor;
            SolidBrush brush = new SolidBrush(lableColor);
            FontStyle tempFS = FontStyle.Regular;
            if (label.Bold)
            {
                tempFS = FontStyle.Bold;
            }
            Font font = new Font(lableFont, labelSize, tempFS);
            StringFormat strFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
            Point p = new Point(lableX, lableY);
            g.DrawString(text, font, brush, p, strFormat);
        }
        //通配字段添加标注
        protected  void AddLabels(CustomLabel label, Graphics g)
        {
            List<double> yValues = data.values;
            string lableText = label.Text;
            if (string.IsNullOrEmpty(lableText))
            {
                return;
            }
            switch (lableText)
            {
                case "#Name#":
                    lableText = stockInfo.Name;
                    AddLabel(lableText, label, g);
                    break;
                case "#Price#":
                    int n = data.numbs.Count;
                    lableText = data.numbs[n - 1].ToString("#0.00");
                    AddLabelC(lableText, label, g);
                    break;
                case "#Interval#":
                    lableText = (data.numbs.Last<Double>() - data.lastClose).ToString("#0.00");
                    AddLabelC(lableText, label, g);
                    break;
                case "#Percent#":
                    lableText = Math.Round((data.numbs.Last<Double>() - data.lastClose) / data.lastClose * 100, 2).ToString();
                    lableText = lableText + "%";
                    AddLabelC(lableText, label, g);
                    break;
                case "#Time#":
                    lableText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    AddLabel(lableText, label, g);
                    break;
                case "#Value1#":
                    lableText = yValues[0].ToString("#0.0");
                    AddValue(lableText, label, g);
                    break;
                case "#Value2#":
                    lableText = yValues[1].ToString("#0.0");
                    AddValue(lableText, label, g);
                    break;
                case "#Value3#":
                    lableText = yValues[2].ToString("#0.0");
                    AddValue(lableText, label, g);
                    break;
                case "#Value4#":
                    lableText = yValues[3].ToString("#0.0");
                    AddValue(lableText, label, g);
                    break;
                case "#Value5#":
                    lableText = yValues[4].ToString("#0.0");
                    AddValue(lableText, label, g);
                    break;
                default:
                    AddLabel(lableText, label, g);
                    break;
            }
        }
        //添加标注
        protected  void AddLabel(string text, CustomLabel label, Graphics g)
        {
            int lableX = label.PositionX;
            int lableY = label.PositionY;
            string lableFont = label.FontType;
            int labelSize = label.FontSize;
            Color lableColor = label.FontColor;
            SolidBrush brush = new SolidBrush(lableColor);
            FontStyle tempFS = FontStyle.Regular;
            if (label.Bold)
            {
                tempFS = FontStyle.Bold;
            }
            Font font = new Font(lableFont, labelSize, tempFS);
            StringFormat strFormat = new StringFormat();
            Point p = new Point(lableX, lableY);
            g.DrawString(text, font, brush, p);
        }
        //标注根据涨跌幅确定颜色
        protected  void AddLabelC(string text, CustomLabel label, Graphics g)
        {
            int lableX = label.PositionX;
            int lableY = label.PositionY;
            string lableFont = label.FontType;
            int labelSize = label.FontSize;
            //根据涨跌确定颜色
            double newValue = data.numbs.Last<Double>();
            double lastClose = data.lastClose;
            Color lableColor;
            if (newValue > lastClose)
            {
                lableColor = Color.Red;
            }
            else
            {
                lableColor = Color.Green;
            }
            SolidBrush brush = new SolidBrush(lableColor);
            FontStyle tempFS = FontStyle.Regular;
            if (label.Bold)
            {
                tempFS = FontStyle.Bold;
            }
            Font font = new Font(lableFont, labelSize, tempFS);
            StringFormat strFormat = new StringFormat();
            Point p = new Point(lableX, lableY);
            g.DrawString(text, font, brush, p);
        }
        //添加直线
        protected  void AddLines(CustomLine line, Graphics g)
        {
            Pen lightBluePen = new Pen(line.LineColor, line.LineSize);
            if (line.LineStyle == "dash")
            {
                lightBluePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            }
            g.DrawLine(lightBluePen, line.StartPoint, line.EndPoint);
        }

    }
}
