using RestSharp;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

            var handle = client.ExecuteAsync(restRequest, (restResponse) => {
                response = restResponse.Content;
                resetEvent.Set();
            });

            resetEvent.WaitOne();

            return response;
        }
    }
}
