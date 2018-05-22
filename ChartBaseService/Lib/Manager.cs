using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChartBaseService
{
    class Manager
    {

        public static Image GetBgImg(int stockId)
        {
            Image img = null;
            string imgFile = Environment.CurrentDirectory + "\\Img\\" + "1.jpg";
            FileInfo imgFileInfo = new FileInfo(imgFile);
            if (imgFileInfo.Exists)
            {
                img = Image.FromFile(imgFile);
                return img;
            }
            return null;
        }
        //写日志
        public static void WriteLog(string level, string file, string sth)
        {
            if (file.EndsWith("\\"))
            {
                file += level + "\\";
            }
            else
            {
                file += "\\" + level + "\\";
            }
            CreatFilePath(file);
            file += DateTime.Now.ToString("yyyyMMdd") + ".log";
            FileStream fs = new FileStream(file, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sw.Write(sth);
            sw.WriteLine();
            sw.WriteLine();
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        public static void SendWeixin(string error, string logPath)
        {
            //string strSendChatUrl= "http://222.73.4.142/QywxInterface/SendChatHandler.ashx?msg={\"receiver\":{ \"type\": \"group\",\"id\": \"61\"},\"sender\": \"ywbz.list@finchina.com\",\"msgtype\": \"text\",\"text\":{\"content\": \"" + errer + "\"" + "}}";            
            string interfacePara = ConfigReader.weiXinAlert + "?" + ConfigReader.weiXinPara;
            string strSendChatUrl = interfacePara.Replace("error", error);
            string result = WXGetResonse(strSendChatUrl, "GET", null, null, null, null);
            if (result.ToLower() == "ok")
            {
                Manager.SyncWriteLog("Produce", logPath, "微信警报发送成功！");
            }
            else
            {
                Manager.SyncWriteLog("Produce", logPath, "微信警报发送失败！");
            }
        }
        #region 取得响应数据
        /// <summary>
        /// 取得响应数据
        /// </summary>
        /// <param name="strUrl">https://qyapi.weixin.qq.com/cgi-bin/message/send</param>
        /// <param name="strMethod">GET/POST</param>
        /// <param name="strUserAgent">Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.94 Safari/537.36</param>
        /// <param name="strAccept">application/json, text/javascript, */*; q=0.01</param>
        /// <param name="strCookieString">xq_a_token=OaQ7ZoMpDvmeAj14Y2YIFL; xq_r_token=lcug6S9na1pY6itSrLghYG; Hm_lvt_1db88642e346389874251b5a1eded6e3=1369621481,1369707448,1369794753,1369893119; Hm_lpvt_1db88642e346389874251b5a1eded6e3=1369894639; __utma=1.297024551.1367823661.1369794753.1369893119.9; __utmb=1.4.10.1369893119; __utmc=1; __utmz=1.1369794753.8.4.utmcsr=baidu|utmccn=(organic)|utmcmd=organic|utmctr=%D1%A9%C7%F2</param> 
        /// <param name="strPostData"></param>
        /// <returns></returns>
        public static string WXGetResonse(string strUrl, string strMethod, string strUserAgent, string strAccept, string strCookieString, string strPostData)
        {
            //响应字符串
            string strResult = string.Empty;

            if (string.IsNullOrEmpty(strUrl))
            {
                return strResult;
            }

            HttpWebRequest Requester = null;

            try
            {
                if (strUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    Requester = WebRequest.Create(strUrl) as HttpWebRequest;
                    Requester.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    Requester = (HttpWebRequest)HttpWebRequest.Create(strUrl);
                }
            }
            catch (Exception ex)
            {
                Manager.SyncWriteLog("Program", ConfigReader.logPath, "微信发送报警信息失败！ \r" + ex);
            }

            if (Requester != null)
            {
                if (string.IsNullOrEmpty(strUserAgent))
                {
                    Requester.UserAgent = @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.94 Safari/537.36";
                }
                else
                {
                    Requester.UserAgent = strUserAgent;
                }

                if (string.IsNullOrEmpty(strAccept))
                {
                    Requester.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                }
                else
                {
                    Requester.Accept = strAccept;
                }

                Requester.ContentType = @"application/x-www-form-urlencoded";

                if (string.IsNullOrEmpty(strMethod) || (strMethod.ToUpper().Trim() != "POST" && strMethod.ToUpper().Trim() != "GET"))
                {
                    //默认GET
                    Requester.Method = "GET";
                }
                else
                {
                    Requester.Method = strMethod;
                }

                if (!string.IsNullOrEmpty(strCookieString))
                {
                    Requester.Headers["Cookie"] = strCookieString;
                }

                if (Requester.Method.ToUpper().Trim() == "POST" && !string.IsNullOrEmpty(strPostData))
                {
                    byte[] data = Encoding.UTF8.GetBytes(strPostData);

                    Requester.ContentLength = data.Length;

                    Stream writeStream = Requester.GetRequestStream();

                    if (writeStream != null)
                    {
                        writeStream.Write(data, 0, data.Length);
                        writeStream.Close();
                    }
                }

                //取得响应数据
                HttpWebResponse Responser = null;
                try
                {
                    Responser = (HttpWebResponse)Requester.GetResponse();
                }
                catch (Exception ex)
                {
                    //LogHelper.InnerError("HttpUtil", "GetResonse:" + ex.ToString());
                    Manager.SyncWriteLog("Program", ConfigReader.logPath, "微信发送报警信息失败！ \r" + ex);
                }

                if (Responser != null)
                {
                    Stream readerStream = Responser.GetResponseStream();

                    if (readerStream != null)
                    {
                        StreamReader reader = new StreamReader(readerStream, Encoding.GetEncoding("utf-8"));

                        if (reader != null)
                        {
                            strResult = reader.ReadToEnd();

                            reader.Close();
                        }

                        readerStream.Close();
                    }

                    Responser.Close();
                }

            }

            return strResult;
        }
        //回调函数
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
        #endregion
        /// <summary>
        /// 报警
        /// </summary>
        /// <param name="id">股票ID</param>
        /// <param name="flag">股票是否报警</param>
        /// <param name="subject">报警标题</param>
        /// <param name="error">报警错误信息</param>
        /// <param name="logPath">日志地址</param>
        public static void ErrorAlert(TaskInfo stockInfo, bool flag, string subject, string error, string logPath)
        {
            if (stockInfo != null)
            {
                flag = stockInfo.Alert;
            }

            if (flag == false)
            {
                SyncWriteLog("Produce", logPath, "警报取消！");
                return;
            }
            subject = "行情文件生成(交易图片.NET)_" + subject;
            if (stockInfo == null)
                error = "\n\r" + ConfigReader.serverName + "报错详细：\n\r" + error;
            else
                error = "\n\r" + ConfigReader.serverName + "\n\rProject=" + stockInfo.Project + " ID=" + stockInfo.Id + " Stock=" + stockInfo.StockCode + "报错详细：\n\r" + error;
            string tempStr = subject + error;
            if (tempStr.Length > 1000)
            {
                tempStr.Substring(0, 1000);
            }
            if (stockInfo == null)
            {
                SendMail(subject, error, logPath);
                SendWeixin(tempStr, logPath);
            }
            else
            {
                SendMail(subject, error, logPath);
                SendWeixin("[衰]" + tempStr, logPath);
            }
        }

        public static void SendMail(string mailSubject, string mailBody, string logPath)
        {
            if (ConfigReader.mailFlag)
            {
                bool flag = SendMail(ConfigReader.mailToAddress.Split(';'), ConfigReader.mailFromAddress, ConfigReader.mailDisplayName, ConfigReader.mailFromPwd, ConfigReader.mailSmtpServer, mailSubject, mailBody);
                if (flag)
                {
                    Manager.SyncWriteLog("Produce", logPath, "邮件警报发送成功！");
                }
                else
                {
                    Manager.SyncWriteLog("Produce", logPath, "邮件警报发送失败！");
                }
            }
        }

        public static bool SendMail(string strMailFromAddress, string strMailDisplayName, string strMailFromPwd, string[] arrMailToAddress, string[] arrMailCcAddress, string[] arrMailBccAddress, string strSmtpServer, string strSubject, string strBody, string[] arrMailAttachment, bool boolIsBodyHtml, MailPriority mailPriority, Encoding mailEncoding, bool Async)
        {
            bool bool_Result = false;

            //发件人地址不能为空
            if (string.IsNullOrEmpty(strMailFromAddress))
            {
                return bool_Result;
            }

            //发件人显示名称为空时，默认显示发件人地址
            if (string.IsNullOrEmpty(strMailDisplayName))
            {
                strMailDisplayName = strMailFromAddress;
            }

            //发件人密码不能为空
            if (string.IsNullOrEmpty(strMailFromPwd))
            {
                return bool_Result;
            }

            //收件人地址不能为空
            if (arrMailToAddress == null)
            {
                return bool_Result;
            }

            //Smtp服务器地址不能为空
            if (string.IsNullOrEmpty(strSmtpServer))
            {
                return bool_Result;
            }

            //邮件标题不能为空
            if (string.IsNullOrEmpty(strSubject))
            {
                return bool_Result;
            }

            //默认邮件编码为 UTF8
            if (mailEncoding == null)
            {
                mailEncoding = System.Text.Encoding.UTF8;
            }

            MailMessage mailMsg = new MailMessage();

            //发件人地址
            try
            {
                mailMsg.From = new MailAddress(strMailFromAddress, strMailDisplayName, mailEncoding);
            }
            catch (Exception)
            {
                return bool_Result;
            }

            //收件人地址
            for (int i = 0; i < arrMailToAddress.Length; i++)
            {
                string temp = arrMailToAddress[i].Trim();
                mailMsg.To.Add(temp);
            }

            //抄送人地址
            if (arrMailCcAddress != null)
            {
                for (int i = 0; i < arrMailCcAddress.Length; i++)
                {
                    mailMsg.CC.Add(arrMailCcAddress[i]);
                }
            }

            //密送人地址
            if (arrMailBccAddress != null)
            {
                for (int i = 0; i < arrMailBccAddress.Length; i++)
                {
                    mailMsg.Bcc.Add(arrMailBccAddress[i]);
                }
            }

            //邮件标题
            mailMsg.Subject = strSubject;

            //邮件标题编码 
            mailMsg.SubjectEncoding = mailEncoding;

            //邮件正文
            mailMsg.Body = strBody;

            //邮件正文编码 
            mailMsg.BodyEncoding = mailEncoding;

            //含有附件
            if (arrMailAttachment != null)
            {
                for (int i = 0; i < arrMailAttachment.Length; i++)
                {
                    mailMsg.Attachments.Add(new Attachment(arrMailAttachment[i]));
                }
            }

            //邮件优先级
            mailMsg.Priority = mailPriority;

            //邮件正文是否是HTML格式
            mailMsg.IsBodyHtml = boolIsBodyHtml;



            SmtpClient smtpClient = new SmtpClient();

            //验证发件人用户名和密码
            smtpClient.Credentials = new System.Net.NetworkCredential(strMailFromAddress, strMailFromPwd);

            //Smtp服务器地址
            smtpClient.Host = strSmtpServer;

            object userState = mailMsg;

            try
            {
                if (Async)
                {
                    //绑定回调函数
                    smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

                    //异步发送邮件
                    smtpClient.SendAsync(mailMsg, userState);
                    bool_Result = true;
                }
                else
                {
                    //同步发送邮件
                    smtpClient.Send(mailMsg);
                    bool_Result = true;
                }
            }
            catch (Exception ex)
            {
                //处理异常
                Manager.SyncWriteLog("Program", ConfigReader.logPath, "邮件发送给" + arrMailToAddress[0] + "...失败！ \r" + ex);
                return false;
            }

            return bool_Result;
        }


        public static bool SendMail(string[] arrMailToAddress, string strMailFromAddress, string strMailDisplayName, string strMailFromPwd, string strSmtpServer, string strSubject, string strBody)
        {
            bool result_bool = false;
            result_bool = SendMail(strMailFromAddress, strMailDisplayName, strMailFromPwd, arrMailToAddress, null, null, strSmtpServer, strSubject, strBody, null, false, MailPriority.High, System.Text.Encoding.UTF8, true);
            return result_bool;

        }
        //回调函数
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            MailMessage mailMsg = (MailMessage)e.UserState;

            if (e.Cancelled)
            {
                //发送被取消
                return;
            }

            if (e.Error != null)
            {
                //出现错误

                return;
            }
        }
        //异步写日志
        public static object locker = new object();
        public static void SyncWriteLog(string level, string file, string sth)
        {
            lock (locker)
            {
                WriteLog(level, file, sth);
            }
        }
        //获取Token
        public static string HttpPostToken(string user, string pwd)
        {
            HttpWebRequest request;
            if (ConfigReader.tokenRequestApi.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = (HttpWebRequest)WebRequest.Create(ConfigReader.tokenRequestApi);
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(ConfigReader.tokenRequestApi);
                request.ProtocolVersion = HttpVersion.Version11;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            }

            string postData = "name=" + user;
            postData += "&pwd=" + pwd;
            byte[] data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;


            request.ProtocolVersion = HttpVersion.Version11;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }
        //更新Token
        public static string HttpGetToken(string refreshToken)
        {
            string param = "?refreshToken=" + refreshToken;
            HttpWebRequest request;

            if (ConfigReader.tokenRequestApi.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = (HttpWebRequest)WebRequest.Create(ConfigReader.tokenRefreshApi + param);
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(ConfigReader.tokenRefreshApi + param);
                request.ProtocolVersion = HttpVersion.Version11;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            }

            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;

        }

        public static bool GetToken()
        {
            string resultJson = Manager.HttpPostToken(ConfigReader.AccountUser, ConfigReader.AccountPwd);
            var DynamicObj = JsonConvert.DeserializeObject<dynamic>(resultJson);
            if (DynamicObj["ok"] == 1)
            {
                TokenCache.Token = DynamicObj["token"];
                TokenCache.RefreshToken = DynamicObj["refreshToken"];
                return true;
            }
            else
            {
                Manager.ErrorAlert(new TaskInfo(), true, "获取Token失败", "获取信息：" + resultJson, ConfigReader.logPath);
                Console.WriteLine("获取Token失败！");
                return false;
            }    
        }

        //private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        //{
        //    return true; //总是接受  
        //}
        //创建文件夹
        public static void CreatFilePath(string path)
        {
            if (path.Substring(path.Length - 1) != "\\")
            {
                path += "\\";
            }
            System.IO.DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                catch (Exception)
                {

                    Console.WriteLine("不能创建" + path + "的文件夹！", "警告");
                    SyncWriteLog("Program", ConfigReader.logPath, "不能创建" + path + "的文件夹！");
                }

            }
        }

    }
}
