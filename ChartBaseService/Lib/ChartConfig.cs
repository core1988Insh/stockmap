using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChartBaseService
{
    class ChartConfig
    {
        public  static void TaskReader()
        {
            TaskInfo taskInfo = new TaskInfo();
            string configFile =AppDomain.CurrentDomain.BaseDirectory + "Config.xml";

            FileInfo ConfigFile = new FileInfo(configFile);
            if (ConfigFile.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configFile);
                XmlNodeList nodeList = xmlDoc.LastChild.LastChild.ChildNodes;
                int count = nodeList.Count;

                for (int i = 0; i < count; i++)
                {
                    taskInfo.Id = i + 1;
                    taskInfo.StockCode = nodeList[i].SelectSingleNode("StockCode").InnerText.Trim();
                    taskInfo.FilePath = nodeList[i].SelectSingleNode("FilePath").InnerText.Trim();
                    taskInfo.LogPath = nodeList[i].SelectSingleNode("LogPath").InnerText.Trim();
                    taskInfo.SaveName = nodeList[i].SelectSingleNode("SaveName").InnerText.Trim();
                    taskInfo.SaveType = nodeList[i].SelectSingleNode("SaveType").InnerText.Trim();
                    taskInfo.Alert = true;
                    taskInfo.Status = "已加载";
                    taskInfo.StartTime = "00:00:00";
                    taskInfo.FinishTime = "00:00:00";
                    taskInfo.Name = nodeList[i].SelectSingleNode("Name").InnerText.Trim();
                    taskInfo.Width = Convert.ToInt32(nodeList[i].SelectSingleNode("Width").InnerText.Trim());
                    taskInfo.Height = Convert.ToInt32(nodeList[i].SelectSingleNode("Height").InnerText.Trim());
                    //读取自定义属性
                    taskInfo.customStockInfo.MoveX = Convert.ToInt32(nodeList[i].SelectSingleNode("MoveX").InnerText.Trim());
                    taskInfo.customStockInfo.MoveY = Convert.ToInt32(nodeList[i].SelectSingleNode("MoveY").InnerText.Trim());
                    taskInfo.customStockInfo.FontSize = Convert.ToInt32(nodeList[i].SelectSingleNode("FontSize").InnerText.Trim());
                    taskInfo.customStockInfo.CurveThickness = Convert.ToInt32(nodeList[i].SelectSingleNode("CurveThickness").InnerText.Trim());
                    taskInfo.customStockInfo.LineThickness = Convert.ToInt32(nodeList[i].SelectSingleNode("LineThickness").InnerText.Trim());
                    taskInfo.customStockInfo.FontType = nodeList[i].SelectSingleNode("FontType").InnerText.Trim();
                    taskInfo.customStockInfo.FontColor = ColorTranslator.FromHtml(nodeList[i].SelectSingleNode("FontColor").InnerText.Trim());
                    taskInfo.customStockInfo.CurveColor = ColorTranslator.FromHtml(nodeList[i].SelectSingleNode("CurveColor").InnerText.Trim());
                    taskInfo.customStockInfo.AxisColor = ColorTranslator.FromHtml(nodeList[i].SelectSingleNode("AxisColor").InnerText.Trim());
                    taskInfo.customStockInfo.TopMargin = Convert.ToInt32(nodeList[i].SelectSingleNode("Top").InnerText.Trim());
                    taskInfo.customStockInfo.BottomMargin = Convert.ToInt32(nodeList[i].SelectSingleNode("Bottom").InnerText.Trim());
                    taskInfo.customStockInfo.LeftMargin = Convert.ToInt32(nodeList[i].SelectSingleNode("Left").InnerText.Trim());
                    taskInfo.customStockInfo.RightMargin = Convert.ToInt32(nodeList[i].SelectSingleNode("Right").InnerText.Trim());
                    taskInfo.customStockInfo.Bold = nodeList[i].SelectSingleNode("Bold").InnerText.Trim();

                    if (string.IsNullOrEmpty(taskInfo.StockCode))
                    {
                        taskInfo.StockCode = "SH000001.index";
                    }
                    if (string.IsNullOrEmpty(taskInfo.FilePath) || taskInfo.FilePath.ToLower() == "default")
                    {
                        taskInfo.FilePath = Environment.CurrentDirectory;
                    }
                    if (string.IsNullOrEmpty(taskInfo.LogPath) || taskInfo.LogPath.ToLower() == "default")
                    {
                        taskInfo.LogPath = Environment.CurrentDirectory;
                    }
                    if (string.IsNullOrEmpty(taskInfo.SaveName))
                    {
                        taskInfo.SaveName = taskInfo.StockCode.Substring(0, 8);
                    }
                    if (string.IsNullOrEmpty(taskInfo.SaveType))
                    {
                        switch (taskInfo.SaveType.ToLower())
                        {
                            case "jpg":
                                taskInfo.SaveType = "jpg";
                                break;
                            case "png":
                                taskInfo.SaveType = "png";
                                break;
                            case "gif":
                                taskInfo.SaveType = "gif";
                                break;
                            case "bmp":
                                taskInfo.SaveType = "bmp";
                                break;
                            default:
                                taskInfo.SaveType = "jpg";
                                break;
                        }
                        taskInfo.SaveType = taskInfo.SaveName.Substring(0, 8);
                    }

                    if (taskInfo.customStockInfo.FontSize == 0)
                    {
                        taskInfo.customStockInfo.FontSize = 1;
                    }
                    if (taskInfo.customStockInfo.CurveThickness == 0)
                    {
                        taskInfo.customStockInfo.CurveThickness = 1;
                    }
                    if (taskInfo.customStockInfo.LineThickness == 0)
                    {
                        taskInfo.customStockInfo.LineThickness = 1;
                    }
                    if (string.IsNullOrEmpty(taskInfo.customStockInfo.FontType))
                    {
                        taskInfo.customStockInfo.FontType = "Calibri";
                    }
                    if (taskInfo.customStockInfo.TopMargin < 1)
                    {
                        taskInfo.customStockInfo.TopMargin = 0;
                    }
                    if (taskInfo.customStockInfo.BottomMargin < 1)
                    {
                        taskInfo.customStockInfo.BottomMargin = 0;
                    }
                    if (taskInfo.customStockInfo.LeftMargin < 1)
                    {
                        taskInfo.customStockInfo.LeftMargin = 0;
                    }
                    if (taskInfo.customStockInfo.RightMargin < 1)
                    {
                        taskInfo.customStockInfo.RightMargin = 0;
                    }
                    if (string.IsNullOrEmpty(taskInfo.customStockInfo.Bold))
                    {
                        taskInfo.customStockInfo.Bold = "false";
                    }
                    if (taskInfo.customStockInfo.Bold.ToLower() != "true")
                    {
                        taskInfo.customStockInfo.Bold = "false";
                    }
                    if (taskInfo.Width != 0)
                    {
                        if (taskInfo.Width < 100)
                        {
                            taskInfo.Width = 100;
                        }
                    }
                    if (taskInfo.Height != 0)
                    {
                        if (taskInfo.Height < 50)
                        {
                            taskInfo.Height = 50;
                        }
                    }

                    //读取Label信息
                    foreach (XmlNode item in nodeList[i].ChildNodes)
                    {
                        if (item.Name == "Label")
                        {
                            ReadLabels(taskInfo, nodeList[i].SelectNodes("Label"));
                            break;
                        }
                    }
                    //读取Label信息
                    foreach (XmlNode item in nodeList[i].ChildNodes)
                    {
                        if (item.Name == "Line")
                        {
                            ReadLines(taskInfo, nodeList[i].SelectNodes("Line"));
                            break;
                        }
                    }
                    //读时间
                    taskInfo.TimeList = ReadLimitTime(nodeList[i].ChildNodes);
                    TaskCache.taskInfo = taskInfo;
                }
                Manager.SyncWriteLog("Program", ConfigReader.logPath, "加载" + configFile + "任务配置文件成功!");                
            }
        }
        //读取Lables
        private static void ReadLabels(TaskInfo taskInfo, XmlNodeList nodeList)
        {

            int n = nodeList.Count;

            for (int i = 0; i < n; i++)
            {
                //添加Label属性
                taskInfo.AddLabels();
                taskInfo.customLabels[i].Text = nodeList[i].SelectSingleNode("Text").InnerText.Trim();
                taskInfo.customLabels[i].FontType = nodeList[i].SelectSingleNode("FontType").InnerText.Trim();
                taskInfo.customLabels[i].PositionX = Convert.ToInt32(nodeList[i].SelectSingleNode("PositionX").InnerText.Trim());
                taskInfo.customLabels[i].PositionY = Convert.ToInt32(nodeList[i].SelectSingleNode("PositionY").InnerText.Trim());
                taskInfo.customLabels[i].FontSize = Convert.ToInt32(nodeList[i].SelectSingleNode("FontSize").InnerText.Trim());
                taskInfo.customLabels[i].FontColor = ColorTranslator.FromHtml(nodeList[i].SelectSingleNode("FontColor").InnerText.Trim());
                string temp = nodeList[i].SelectSingleNode("Bold").InnerText.Trim();
                if (temp.ToLower() != "true")
                {
                    taskInfo.customLabels[i].Bold = false;
                }
                else
                {
                    taskInfo.customLabels[i].Bold = true;
                }
            }
        }
        //读取Lines
        private static void ReadLines(TaskInfo taskInfo, XmlNodeList nodeList)
        {
            int n = nodeList.Count;
            for (int i = 0; i < n; i++)
            {
                //添加Label属性
                taskInfo.AddLines();
                taskInfo.customLines[i].LineStyle = nodeList[i].SelectSingleNode("LineStyle").InnerText.Trim();
                taskInfo.customLines[i].StartPoint = new Point(Convert.ToInt32(nodeList[i].SelectSingleNode("StartPointX").InnerText.Trim()), Convert.ToInt32(nodeList[i].SelectSingleNode("StartPointY").InnerText.Trim()));
                taskInfo.customLines[i].EndPoint = new Point(Convert.ToInt32(nodeList[i].SelectSingleNode("EndPointX").InnerText.Trim()), Convert.ToInt32(nodeList[i].SelectSingleNode("EndPointY").InnerText.Trim()));
                taskInfo.customLines[i].LineSize = Convert.ToInt32(nodeList[i].SelectSingleNode("LineSize").InnerText.Trim());
                taskInfo.customLines[i].LineColor = ColorTranslator.FromHtml(nodeList[i].SelectSingleNode("LineColor").InnerText.Trim());
                string temp = nodeList[i].SelectSingleNode("LineStyle").InnerText.Trim();
                if (temp.ToLower() != "dash")
                {
                    taskInfo.customLines[i].LineStyle = "solid";
                }
                else
                {
                    taskInfo.customLines[i].LineStyle = "dash";
                }
            }
        }

        //读取运行时间
        private static int[] ReadLimitTime(XmlNodeList xmlList)
        {
            int[] intArray = new int[4];
            string[] strArray = new string[4];
            foreach (XmlNode node in xmlList)
            {
                if (node.Name == "TradeTime")
                {
                    strArray = node.InnerText.Trim().Split(new char[] { ',' });
                }
            }

            try
            {
                intArray[0] = Convert.ToInt32(strArray[0]);
                intArray[1] = Convert.ToInt32(strArray[1]);
                intArray[2] = Convert.ToInt32(strArray[2]);
                intArray[3] = Convert.ToInt32(strArray[3]);
                return intArray;
            }
            catch (Exception)
            {
                //提示错误
                intArray = new int[] { 930, 1130, 1300, 1500 };
                return intArray;
            }

        }

    }
}
