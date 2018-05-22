using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace ChartBaseService
{
    public class WebServer
    {
       public  void WebServerRun()
        {    //加载主配置信息
            ConfigReader.ReadConfig();
            //加载任务配置信息
            ChartConfig.TaskReader();
            //获取Token
            Manager.GetToken();
            //后台线程轮询获取Token
            Task tRefreshTokens = new Task(TokenCircle, TaskCreationOptions.LongRunning);
            tRefreshTokens.Start();
            //启动API
            APIStart();
        }

        private  void APIStart()
        {
            var config = new HttpSelfHostConfiguration("http://localhost:" + ConfigReader.apiPort);
            config.Routes.MapHttpRoute("default", "{controller}/{id}", new { controller = "Chart", id = RouteParameter.Optional });
            var server = new HttpSelfHostServer(config);
            var task = server.OpenAsync();
            task.Wait();
            Manager.SyncWriteLog("program",ConfigReader.logPath,"WebServer Is Running!");           

        }
        private static void TokenCircle()
        {
            while (true)
            {
                Thread.Sleep(30 * 60 * 1000);
                bool result = Manager.GetToken();
                if (!result)
                {
                    result = Manager.GetToken();
                    if (!result)
                    {
                        result = Manager.GetToken();
                    }
                }
            }
        }
    }
}
