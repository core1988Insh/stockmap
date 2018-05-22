using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChartBaseService
{
    class ChartData
    {

        //昨收价
        public double lastClose;
        //最大值
        public double max;
        //最小值
        public double min;
        //Y轴值
        public List<double> values;
        //分时图的数据值
        public List<double> numbs;
        //行情数据表
        public DataTable dt = new DataTable();
        //私有变量
        protected int _id;
        protected string _stockCode;
        protected string _logPath;
        protected bool _alertFlag;
        protected TaskInfo _stockInfo;

        public  void InitiData(TaskInfo stockInfo)
        {
            _id = stockInfo.Id;
            _stockCode = stockInfo.StockCode;
            _logPath = stockInfo.LogPath;
            _alertFlag = stockInfo.Alert;
            _stockInfo = stockInfo;
        }

        public  void AddData()
        {
            //预加载昨收数据不存在则重新获取
          
             GetLastClosePoint(_stockCode);
        
            if (lastClose == 0)
            {
                //没有昨收数据
                values = new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                numbs = null;
                Manager.SyncWriteLog("Produce", _logPath, "股票" + _stockCode + "没有获取昨收数据!");
                Manager.ErrorAlert(_stockInfo, _alertFlag, "数据错误！ ", "股票" + _stockCode + "未获取昨收数据，不能生成行情图片！", _logPath);
                return;
            }
            GetDatas(_stockCode);            
            GetMaxAndMin(numbs, lastClose);
            values = new List<double> { max,Math.Round((3*max+lastClose)/4,2), Math.Round((max+lastClose)/2,2),Math.Round((max+3*lastClose)/4,2),lastClose,Math.Round((min+3*lastClose)/4,2),
                 Math.Round((min+lastClose)/2,2),Math.Round((3*min+lastClose)/4,2), min};
        }

        /// <summary>
        /// 获取开盘价的json,K线图需要的最新数据
        /// </summary>
        /// <param name="stockCode">股票代码</param>
        /// <returns></returns>
        private string GetLastCloseJson(string stockCode)
        {
            string lastCloseJson = null;
            string path = ConfigReader.quotePriceApi + "?objs=" + stockCode + "&fields=lastclose&token=" + TokenCache.Token;
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(path);
                StreamReader sr = new StreamReader(stream);
                lastCloseJson = sr.ReadToEnd();
                sr.Close();
                client.Dispose();
                if (!lastCloseJson.Contains("\"ok\":1"))
                {
                    lastClose = 0;
                    Manager.SyncWriteLog("Produce", _logPath, "访问" + path + " 返回信息为:" + lastCloseJson);
                    Manager.ErrorAlert(_stockInfo, _alertFlag,  "未获取昨收数据！", "访问" + path + "返回信息：" + lastCloseJson, _logPath);
                    return null;
                }
                return lastCloseJson;
            }
            catch (Exception ex)
            {
                lastClose = 0;
                Manager.SyncWriteLog("Produce", _logPath, "访问" + path + "异常，请检查网络访问是否正常？   " + ex);
                Manager.ErrorAlert(_stockInfo, _alertFlag, "接口访问网络异常！", "访问" + path + "异常，请检查网络访问是否正常？   " + ex, _logPath);
                return null;
            }
            finally { }

        }


        /// <summary>
        /// 获取今天的json数据
        /// </summary>
        /// <param name="begintime">今天的时间</param>
        /// <param name="endtime">今天的时间</param>
        /// <param name="type">1min</param>
        /// <param name="objstr">股票代码</param>
        /// <param name="pathstr">webAPI</param>
        /// <param name="fields">数据字段</param>
        /// <returns></returns>
        private string GetTodayJson(string begintime, string endtime, string objstr, string pathstr, string type = "1min", string fields = "close")
        {
            string minutesJson = null;
            string path = pathstr + "?obj=" + objstr + "&begin=" + begintime + "0900&end=" + endtime + "1500&fields=" + fields + "&type=" + type + "&token=" + TokenCache.Token;
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(path);
                StreamReader sr = new StreamReader(stream);
                minutesJson = sr.ReadToEnd();
                sr.Close();
                client.Dispose();
                //Manager.SyncWriteLog("Produce", _logPath, "访问" + path + " 返回信息为:" + minutesJson);
                if (!minutesJson.Contains("\"ok\":1"))
                {
                    numbs = null;
                    Manager.SyncWriteLog("Produce", _logPath, "访问" + path + " 返回信息为:" + minutesJson);
                    Manager.ErrorAlert(_stockInfo, _alertFlag, "行情接口数据错误！未正常获取行情数据！", "访问" + path + " 返回信息为:" + minutesJson, _logPath);
                    return null;
                }
                return minutesJson;
            }
            catch (Exception ex)
            {
                Manager.SyncWriteLog("Produce", _logPath, "访问" + path + "异常，请检查网络访问是否正常？  " + ex);
                Manager.ErrorAlert(_stockInfo, _alertFlag, "行情接口访问异常 ,接口访问网络异常！", "访问" + path + "异常，请检查网络访问是否正常？  " + ex, _logPath);
                return null;
            }
            finally { }

        }
        //获取昨收价
        private void GetLastClosePoint(string stockCode)
        {
            string lastCloseJson = null;
            lastCloseJson = GetLastCloseJson(stockCode);
            if (string.IsNullOrEmpty(lastCloseJson))
            {
                lastClose = 0;
                return;
            }
            string lastCloseStr = lastCloseJson.Substring(lastCloseJson.IndexOf("[[") + 2, lastCloseJson.LastIndexOf("]]") - lastCloseJson.IndexOf("[[") - 2);
            //没有数据
            if (string.IsNullOrEmpty(lastCloseStr))
            {
                lastClose = 0;
                Manager.SyncWriteLog("Produce", _logPath, "访问" + stockCode + " 昨收接口返回信息为:" + lastCloseJson);
                Manager.ErrorAlert(_stockInfo, _alertFlag, "昨收数据异常 !访问" + stockCode + "昨收接口返回信息：NUll", "访问" + stockCode + "昨收接口返回信息为：" + lastCloseJson, _logPath);
                return;
            }
            if (lastCloseStr == "0")
            {
                lastClose = 0;
                Manager.SyncWriteLog("Produce", _logPath, "访问" + stockCode + " 昨收接口返回信息：" + lastCloseJson);
                Manager.ErrorAlert(_stockInfo, _alertFlag, "昨收数据异常 !未获取昨收数据！", "访问" + stockCode + "昨收接口返回信息：" + lastCloseJson, _logPath);
                return;
            }
            try
            {
                lastClose = Math.Round(Convert.ToDouble(lastCloseStr), 2);
            }
            catch (Exception ex)
            {
                lastClose = 0;
                Manager.SyncWriteLog("Produce", _logPath, "访问" + ConfigReader.quotePriceApi + "?objs=" + stockCode + " 返回数据异常：" + ex);
           
            }
        }
        //获取分时图数据
        private void GetDatas(string stockCode)
        {
            numbs = new List<double>();
            //处理Json数据
            string today = DateTime.Now.ToString("yyyyMMdd");
            string dataStr = GetTodayJson(today, today, stockCode, ConfigReader.marketPriceApi);
            //没有获取信息
            if (string.IsNullOrEmpty(dataStr))
            {
                numbs = null;  return;
            }
            string tempStr = dataStr.Substring(dataStr.IndexOf("[[") + 1, dataStr.LastIndexOf("]]") - dataStr.IndexOf("[[")).Replace("[", "").Replace("]", "");
            //没有数据
            if (string.IsNullOrEmpty(tempStr))
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday||DateTime.Now.DayOfWeek==DayOfWeek.Saturday)
                {
                    numbs = null; return;
                }
                TimeSpan ts= DateTime.Now - DateTime.ParseExact("0900", "HHmm", System.Globalization.CultureInfo.CurrentCulture);
                int temp = (int)ts.TotalMinutes;
                if (temp>0&& temp < 150) {

                    for (int i = 0; i < temp; i++)
                    {
                        numbs.Add(Convert.ToDouble(lastClose));
                    }
                } 
               else if (temp > 150&&temp<360)
                {
                    temp = temp - 120;
                    for (int i = 0; i < temp; i++)
                    {
                        numbs.Add(Convert.ToDouble(lastClose));
                    }
                };
                return;
            }
            string[] datas = tempStr.Split(new char[] { ',' });
            //有数据
            int count = datas.Length;
            for (int i = 0; i < count; i++)
            {
                numbs.Add(Convert.ToDouble(datas[i]));
            }
        }

        //获取最大最小值
        private void GetMaxAndMin(List<double> numbs, double lastClose)
        {
            if (lastClose == 0)
            {
                max = 0;
                min = 0;
            }
            if (numbs != null)
            {
                max = numbs.Max();
                min = numbs.Min();
                if (max == min)
                {
                    max = Math.Round(max * 1.002, 2);
                    min = Math.Round(min * 0.998, 2);
                    return;
                }

                if (Math.Abs(max - lastClose) > Math.Abs(min - lastClose))
                {
                    max = Math.Round(max * 1.002, 2);
                    min = Math.Round((2 * lastClose - max), 2);
                }
                else
                {
                    min = Math.Round(min * 0.998, 2);
                    max = Math.Round(2 * lastClose - min, 2);
                }

            }
            else
            {
                max = Math.Round(lastClose * 1.05, 2);
                min = Math.Round(lastClose * 0.95, 2);
            }
        }
    }
}
