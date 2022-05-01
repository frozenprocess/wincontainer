using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace wincontainer.Controllers
{
    public class CrawlerController : Controller
    {
        // GET: /Crawler/

        public async Task<string> Index()
        {
            string url = "https://www.githubstatus.com/";
            if (Environment.GetEnvironmentVariable("HTTP_URL") is not null){
                url = Environment.GetEnvironmentVariable("HTTP_URL");
            }
            return await CrawlerAsync(url);
        }

        // 
        // GET: /Crawler/Welcome/ 

        public async Task<string> Welcome()
        {
            string url = string.Format("https://{0}:{1}",Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST"),Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT"));
            return await CrawlerAsync(url);
        }

        private async Task<string> CrawlerAsync(string uri)
        {
            int response = 0;
            int timeout = 5;
            if (Environment.GetEnvironmentVariable("HTTP_TIMEOUT") is not null)
            {
                timeout = Int32.Parse(Environment.GetEnvironmentVariable("HTTP_TIMEOUT"));
            }
            HttpClient client = new HttpClient();
            try
            {
                client.DefaultRequestHeaders.ConnectionClose = true;
                client.Timeout = TimeSpan.FromSeconds(timeout);

                var msg = new HttpRequestMessage(HttpMethod.Get, uri);
                var responseTask = await client.SendAsync(msg);

                response = (int)responseTask.StatusCode;
            }
            catch (HttpRequestException e)
            {
                response = 499;
                Console.WriteLine(e.Message);
            }
            finally
            {
                client.Dispose();
            }
            return response.ToString();
        }

    }
}
