﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChartBaseService;

namespace WebServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServer webServer = new WebServer();
            webServer.WebServerRun();
            Console.ReadLine();
            Console.ReadKey();
        }
    }
}
