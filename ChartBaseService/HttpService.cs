using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ChartBaseService
{
    public partial class HttpService : ServiceBase
    {
        public HttpService()
        {
            InitializeComponent();
        }
        WebServer webserver = new WebServer();
        protected override void OnStart(string[] args)
        {
            webserver.WebServerRun();
        }

        protected override void OnStop()
        {
        }
    }
}
