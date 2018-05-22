using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChartBaseService
{
    class ConfigReader
    {

        //默认配置
        public static string logPath = AppDomain.CurrentDomain.BaseDirectory + "Logs";
        public static string filePath = AppDomain.CurrentDomain.BaseDirectory;
        public static string stockCode = "SH000001.index";
        public static string serverName = System.Environment.MachineName;

        public static void ReadConfig()
        {
            try
            {
                ReadServerName();
                ReadApiPort();
                AccountConfig();
                InterFaceConfig();
                MailConfig();
            }
            catch (Exception ex)
            {
                Manager.SyncWriteLog("Program", logPath, "读取配置信息错误：" + ex);
                Console.WriteLine("配置文件不存在，或配置错误！");
                Environment.Exit(0);
            }
            Manager.SyncWriteLog("Program", logPath, "读取主配置信息成功!");

        }

        //获取ServerName
        private static void ReadServerName()
        {
            string configFile = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";
            FileInfo ConfigFile = new FileInfo(configFile);
            if (ConfigFile.Exists)
            {
                serverName = XmlReaderCL(configFile, "Root/ServerName");
            }
            if (string.IsNullOrEmpty(serverName))
            {
                serverName = System.Environment.MachineName;
            }
        }

        public static int apiPort = 8098;
        //读取Api端口
        private static void ReadApiPort()
        {
            string configFile = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";
            FileInfo ConfigFile = new FileInfo(configFile);
            string tempApiPort = null ;
            if (ConfigFile.Exists)
            {
                tempApiPort = XmlReaderCL(configFile, "Root/ApiPort");
            }
            if (!string.IsNullOrEmpty(tempApiPort))
            {
                try { apiPort = Convert.ToInt32(tempApiPort); }
                catch (Exception) { }
            }
        }

        public static string marketPriceApi;
        public static string quotePriceApi;
        public static string weiXinAlert;
        public static string weiXinPara;
        public static string tradeDateConStr;
        public static string tokenRequestApi;
        public static string tokenRefreshApi;
        //读取接口信息
        private static void InterFaceConfig()
        {
            string configFile = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";
            FileInfo ConfigFile = new FileInfo(configFile);
            if (ConfigFile.Exists)
            {
                marketPriceApi = XmlReaderCL(configFile, "Root/InterfaceConfig/MarketPrice");
                quotePriceApi = XmlReaderCL(configFile, "Root/InterfaceConfig/QuotePrice");
                weiXinAlert = XmlReaderCL(configFile, "Root/InterfaceConfig/WXInterface");
                weiXinPara = XmlReaderCL(configFile, "Root/InterfaceConfig/WXParameter");
                tradeDateConStr = XmlReaderCL(configFile, "Root/InterfaceConfig/SqlConnection");
                tokenRequestApi = XmlReaderCL(configFile, "Root/InterfaceConfig/TokenRequest");
                tokenRefreshApi = XmlReaderCL(configFile, "Root/InterfaceConfig/TokenRefresh");
            }
        }

        public static bool mailFlag;
        public static string mailSwitch;
        public static string mailFromAddress;
        public static string mailDisplayName;
        public static string mailFromPwd;
        public static string mailSmtpServer;
        public static string mailToAddress;
        //读取邮件信息
        private static void MailConfig()
        {
            string configFile = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";
            FileInfo ConfigFile = new FileInfo(configFile);
            if (ConfigFile.Exists)
            {
                mailSwitch = XmlReaderCL(configFile, "Root/MailConfig/Switch");
                mailFromAddress = XmlReaderCL(configFile, "Root/MailConfig/MailFromAddress");
                mailDisplayName = XmlReaderCL(configFile, "Root/MailConfig/MailDisplayName");
                mailFromPwd = XmlReaderCL(configFile, "Root/MailConfig/MailFromPwd");
                mailSmtpServer = XmlReaderCL(configFile, "Root/MailConfig/SmtpServer");
                mailToAddress = XmlReaderCL(configFile, "Root/MailConfig/MailToAddress");
                if (mailSwitch.ToLower() == "on")
                {
                    mailFlag = true;
                    if (string.IsNullOrEmpty(mailFromAddress))
                    {
                        mailFlag = false;
                        Manager.SyncWriteLog("Program", logPath, "邮件发件人信息为空！");
                    }
                    if (string.IsNullOrEmpty(mailFromPwd))
                    {
                        mailFlag = false;
                        Manager.SyncWriteLog("Program", logPath, "邮件发件人密码为空！");
                    }
                    if (string.IsNullOrEmpty(mailSmtpServer))
                    {
                        mailFlag = false;
                        Manager.SyncWriteLog("Program", logPath, "邮件服务器信息为空！");
                    }
                    if (string.IsNullOrEmpty(mailToAddress))
                    {
                        mailFlag = false;
                        Manager.SyncWriteLog("Program", logPath, "邮件接收人信息为空！");
                    }
                }
                else
                {
                    mailFlag = false;
                }
            }

        }

        public static string AccountUser;

        public static string AccountPwd;
        //Token接口的认证账户信息
        private static void AccountConfig()
        {
            string configFile = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";
            FileInfo ConfigFile = new FileInfo(configFile);
            if (ConfigFile.Exists)
            {
                AccountUser = XmlReaderCL(configFile, "Root/ApiAccount/User/Name");
                AccountPwd = XmlReaderCL(configFile, "Root/ApiAccount/User/Password");
            }
        }

        /// <summary>
        /// 加载XML文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static XmlDocument LondXmlDoc(string filePath)
        {
            //文件是否存在
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists)
            {
                return null;
            }
            //加载XML文件 
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            return xmlDoc;
        }
        /// <summary>
        /// 查找指定Xml文件的目标Node的信息
        /// </summary>
        /// <param name="xmlDoc">xml文件</param>
        /// <param name="nodePath">目标node的路径</param>
        /// <returns></returns>
        public static string XmlReaderCL(XmlDocument xmlDoc, string nodePath)
        {
            string text = null;
            if (xmlDoc == null)
            {
                return text;
            }
            XmlNodeList xNodeList = xmlDoc.ChildNodes;
            string[] xPath = nodePath.Split(new char[] { '/' });

            //获取根目录
            XmlNode xNode = null;
            foreach (XmlNode item in xNodeList)
            {
                if (item.Name == xPath[0])
                {
                    xNode = item;
                }

            }
            if (xNode == null)
            {
                return text;
            }

            //获取其他路径
            int length = xPath.Count();
            for (int i = 1; i < length; i++)
            {
                xNodeList = xNode.ChildNodes;
                XmlNode temp = null;
                foreach (XmlNode item in xNodeList)
                {
                    if (item.Name == xPath[i])
                    {
                        temp = item;
                        xNode = temp;
                        break;
                    }
                }
                //判断本次循环是否查询到内容
                if (temp == null)
                {
                    return text;
                }
            }

            text = xNode.InnerText.Trim();
            return text;
        }

        /// <summary>
        /// XML文件中读取某一个节点的值，
        /// 节点如果存在读取其中Value，不存在返回Null
        /// </summary>
        /// <param name="filePath">Xml文件的路径</param>
        /// <param name="nodePath">节点的路径</param>
        /// <returns>返回读取的节点值</returns>
        public static string XmlReaderCL(string filePath, string nodePath)
        {
            string text = null;
            //文件是否存在
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists)
            {
                return text;
            }
            //加载XML文件
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList xNodeList = xmlDoc.ChildNodes;
            string[] xPath = nodePath.Split(new char[] { '/' });

            //获取根目录
            XmlNode xNode = null;
            foreach (XmlNode item in xNodeList)
            {
                if (item.Name == xPath[0])
                {
                    xNode = item;
                }

            }
            if (xNode == null)
            {
                return text;
            }

            //获取其他路径
            int length = xPath.Count();
            for (int i = 1; i < length; i++)
            {
                xNodeList = xNode.ChildNodes;
                XmlNode temp = null;
                foreach (XmlNode item in xNodeList)
                {
                    if (item.Name == xPath[i])
                    {
                        temp = item;
                        xNode = temp;
                        break;
                    }
                }
                //判断本次循环是否查询到内容
                if (temp == null)
                {
                    return text;
                }
            }

            text = xNode.InnerText.Trim();
            return text;
        }

        /// <summary>
        /// XMl文件中找到一个目标Node
        /// </summary>
        /// <param name="filePath">xml文件路径</param>
        /// <param name="nodePath">目标节点路径</param>
        /// <returns></returns>
        public static XmlNode XmlNodeFind(XmlDocument xmlDoc, string nodePath)
        {
            XmlNodeList xNodeList = xmlDoc.ChildNodes;
            string[] xPath = nodePath.Split(new char[] { '/' });

            //获取根目录
            XmlNode flagNode = null;
            foreach (XmlNode item in xNodeList)
            {
                if (item.Name == xPath[0])
                {
                    flagNode = item;
                }

            }
            if (flagNode == null)
            {
                return null;
            }

            //获取其他路径
            int length = xPath.Count();
            for (int i = 1; i < length; i++)
            {
                xNodeList = flagNode.ChildNodes;
                XmlNode temp = null;
                foreach (XmlNode item in xNodeList)
                {
                    if (item.Name == xPath[i])
                    {
                        temp = item;
                        flagNode = temp;
                        break;
                    }
                }
                //判断本次循环是否查询到内容
                if (temp == null)
                {
                    return null;
                }
            }

            return flagNode;
        }

        /// <summary>
        /// 读取某Node节点下子节点内容
        /// </summary>
        /// <param name="node">父节点</param>
        /// <param name="nodeChildPath">子节点的相对路径</param>
        /// <returns></returns>
        public static string NodeReaderCL(XmlNode node, string nodeChildPath)
        {
            string[] xNodes = nodeChildPath.Split(new char[] { '/' });

            XmlNode flagNode = node;
            int count = xNodes.Length;
            if (xNodes[0] == node.Name)
            {
                for (int i = 1; i < count; i++)
                {
                    XmlNodeList xNodeList = flagNode.ChildNodes;
                    XmlNode temp = null;
                    foreach (XmlNode item in xNodeList)
                    {
                        if (item.Name == xNodes[i])
                        {
                            temp = item;
                            flagNode = temp;
                            break;
                        }
                    }
                    if (temp == null)
                    {
                        return null;
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    XmlNodeList xNodeList = flagNode.ChildNodes;
                    XmlNode temp = null;
                    foreach (XmlNode item in xNodeList)
                    {
                        if (item.Name == xNodes[i])
                        {
                            temp = item;
                            flagNode = temp;
                            break;
                        }
                    }
                    if (temp == null)
                    {
                        return null;
                    }
                }
            }
            return flagNode.InnerText.Trim();
        }
        //自定义日志路径
        private static void LogPathReader()
        {

            string configFile = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";
            FileInfo ConfigFile = new FileInfo(configFile);
            if (ConfigFile.Exists)
            {
                try
                {
                    logPath = XmlReaderCL(configFile, "Root/LogPath");
                    if (string.IsNullOrWhiteSpace(logPath))
                    {
                        logPath = Environment.CurrentDirectory + "\\Logs";
                    }
                }
                catch (Exception) { }
            }
        }

        public static bool PackSwith;
        public static string PackSource;
        public static string PackTarget;
        public static string PackLog;
        //读取压缩信息
        public static void ZipPackReader()
        {
            string configFile = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";
            FileInfo ConfigFile = new FileInfo(configFile);
            if (ConfigFile.Exists)
            {
                PackSwith = false;
                try
                {
                    string swith = XmlReaderCL(configFile, "Root/ZipPack/Swich");
                    if (swith.ToLower() == "on")
                        PackSwith = true;
                    PackSource = XmlReaderCL(configFile, "Root/ZipPack/Source");
                    PackTarget = XmlReaderCL(configFile, "Root/ZipPack/Target");
                    PackLog = XmlReaderCL(configFile, "Root/ZipPack/Log");
                }
                catch (Exception) { }
            }
        }

        public static int cycleTime = 1;
        //自定义任务循环时间
        private static void CycleTime()
        {
            string configFile = AppDomain.CurrentDomain.BaseDirectory + "Config.xml";
            FileInfo ConfigFile = new FileInfo(configFile);
            if (ConfigFile.Exists)
            {
                try
                {
                    string temp = XmlReaderCL(configFile, "Root/CycleTime");
                    if (string.IsNullOrWhiteSpace(temp))
                        temp = "1";
                    cycleTime = Convert.ToInt32(temp);
                }
                catch (Exception)
                {
                    cycleTime = 1;
                }
            }
        }



    }
}
