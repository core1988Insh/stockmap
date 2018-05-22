using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ChartBaseService
{
    public class ChartController : ApiController
    {
       
        public HttpResponseMessage Get(string stockCode)
        {
            TaskCache.taskInfo.StockCode = stockCode;
            TaskCache.taskInfo.SaveName = stockCode;
            ChartTask task = new ChartTask();
            MemoryStream ms = new MemoryStream();
            Image img = task.Run();
            if (img == null)
            {
                HttpResponseMessage error = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                error.Content = new ByteArrayContent(System.Text.Encoding.Default.GetBytes("Error"));
                return error;
            }
            img.Save(ms,ImageFormat.Png);
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new ByteArrayContent(ms.GetBuffer());
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            Manager.SyncWriteLog("Produce", TaskCache.taskInfo.LogPath, "request:StockCode = " + TaskCache.taskInfo.StockCode+",responce success!");
            return resp;
        }
        public HttpResponseMessage Get(string stockCode,string timeStamp)
        {
            TaskCache.taskInfo.StockCode = stockCode;
            TaskCache.taskInfo.SaveName = stockCode;
            ChartTask task = new ChartTask();
            MemoryStream ms = new MemoryStream();
            Image img = task.Run();
            if (img == null)
            {
                HttpResponseMessage error = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                error.Content = new ByteArrayContent(System.Text.Encoding.Default.GetBytes("Error"));
                return error;
            }
            img.Save(ms, ImageFormat.Png);
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new ByteArrayContent(ms.GetBuffer());
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            Manager.SyncWriteLog("Produce", TaskCache.taskInfo.LogPath, "request:StockCode = " + TaskCache.taskInfo.StockCode+ "and tiemStamp = " +timeStamp+ ",responce success!");
            return resp;
        }

        public string Get()
        {
            return "HttpServer Of ChartBase Is Ready!" ;
        }
    }
}
