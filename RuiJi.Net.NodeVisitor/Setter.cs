using RestSharp;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuiJi.Net.NodeVisitor
{
    public class Setter
    {
        public static string GetRandomSettingUA()
        {
            var proxyUrl = "";

            if (RuiJiConfiguration.Standalone)
            {
                proxyUrl = RuiJiConfiguration.RuiJiServer;
            }
            else
            {
                proxyUrl = ProxyManager.Instance.Elect(NodeProxyTypeEnum.FEEDPROXY);
            }

            if (string.IsNullOrEmpty(proxyUrl))
                throw new Exception("get feedjobs: proxyUrl can't be null");

            proxyUrl = IPHelper.FixLocalUrl(proxyUrl);

            var client = new RestClient("http://" + proxyUrl);
            var restRequest = new RestRequest("api/setting/ua/random");
            restRequest.Method = Method.GET;
            restRequest.Timeout = 15000;

            string response = "";
            var resetEvent = new ManualResetEvent(false);

            // 这里服务端返回了 500，导致返回的 UA 是一串 HTML
            var handle = client.ExecuteAsync(restRequest, (restResponse) =>
            {
                if (restResponse.StatusCode == HttpStatusCode.OK)
                {
                    response = restResponse.Content;
                }
                else
                {
                    // 如果服务端返回了失败的结果
                    response =
                        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
                }
                resetEvent.Set();
            });

            // 可以全部改成 Task 异步
            resetEvent.WaitOne();

            return response;
        }
    }
}