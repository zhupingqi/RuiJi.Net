using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Compile;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Crawler;
using RuiJi.Net.Core.Utils;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Db;
using RuiJi.Net.NodeVisitor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace RuiJi.Net.Owin.Controllers
{
    public class SettingApiController : ApiController
    {
        #region 节点设置
        [HttpGet]
        public object Nodes()
        {
            var feeds = ServerManager.Get(Node.NodeTypeEnum.FEED);
            var crawlers = ServerManager.Get(Node.NodeTypeEnum.CRAWLER);
            return new
            {
                feeds = feeds,
                crawlers = crawlers
            };
        }

        #endregion

        #region 节点函数
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object Funcs(int offset, int limit)
        {
            var node = ServerManager.Get(Request.RequestUri.Authority);

            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            var list = FuncLiteDb.GetModels(paging);

            return new
            {
                rows = list,
                total = list.Count
            };
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object GetFunc(int id)
        {
            return FuncLiteDb.Get(id);
        }

        [HttpPost]
        public object FuncTest(FuncModel func)
        {
            var code = "{# " + func.Sample + " #}";
            var test = new ComplieFuncTest(func.Code);
            return test.GetResult(code);
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object UpdateFunc(FuncModel func)
        {
            if (func.Name == "now" || func.Name == "ticks")
                return false;

            var f = FuncLiteDb.Get(func.Name);
            if (f != null && f.Id == 0)
                return false;

            FuncLiteDb.AddOrUpdate(func);
            return true;
        }

        [HttpGet]
        public bool RemoveFunc(string ids)
        {
            var removes = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(m => Convert.ToInt32(m)).ToArray();

            return FuncLiteDb.Remove(removes);
        }
        #endregion

        #region Proxys
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object Proxys(int offset, int limit)
        {
            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            return new
            {
                rows = ProxyLiteDb.GetModels(paging),
                total = paging.Count
            };
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object UpdateProxy(ProxyModel proxy)
        {
            ProxyLiteDb.AddOrUpdate(proxy);

            return true;
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object GetProxy(int id)
        {
            var feed = ProxyLiteDb.Get(id);

            return feed;
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public bool RemoveProxy(string ids)
        {
            var removes = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(m => Convert.ToInt32(m)).ToArray();

            return ProxyLiteDb.Remove(removes);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public bool ProxyStatusChange(string ids, string status)
        {
            var changeIds = ids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
            var statusEnum = (Status)Enum.Parse(typeof(Status), status.ToUpper());

            return ProxyLiteDb.StatusChange(changeIds, statusEnum);
        }


        [HttpGet]
        public object ProxyPing(int id)
        {
            var watch = new Stopwatch();
            watch.Start();

            try
            {

                var crawler = new RuiJiCrawler();
                var request = new Request("https://www.baidu.com/");
                request.Timeout = 15000;

                var proxy = ProxyLiteDb.Get(id);
                request.Proxy = new RequestProxy(proxy.Ip, proxy.Port, proxy.UserName, proxy.Password);
                request.Proxy.Scheme = proxy.Type == ProxyTypeEnum.HTTP ? "http" : "https";

                var response = crawler.Request(request);

                watch.Stop();

                return new
                {
                    elspsed = watch.Elapsed.Milliseconds,
                    code = response.StatusCode,
                    msg = response.StatusCode.ToString()
                };
            }
            catch (Exception ex)
            {
                watch.Stop();

                return new
                {
                    elspsed = watch.Elapsed.Milliseconds,
                    code = -1,
                    msg = ex.Message
                };
            }
        }

        [HttpGet]
        public object Ping()
        {
            var request = ((Microsoft.Owin.OwinContext)Request.Properties["MS_OwinContext"]).Request;

            return new
            {
                Uri = request.Uri,
                RemoteIpAddress = request.RemoteIpAddress,
                RemotePort = request.RemotePort.Value,
                LocalIpAddress = request.LocalIpAddress,
                LocalPort = request.LocalPort,

            };
        }
        #endregion

        #region uaGroup
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object UAGroups()
        {
            return UAGroupLiteDb.GetModels();
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object UAGroup(int id)
        {
            return UAGroupLiteDb.Get(id);
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public int UpdateUAGroup(UAGroupModel group)
        {
            return UAGroupLiteDb.AddOrUpdate(group);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public bool RemoveUAGroup(int id)
        {
            return UALiteDb.RemoveByGorup(id) ? UAGroupLiteDb.Remove(id) : false;
        }
        #endregion

        #region ua
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object UAs(int offset, int limit, int groupId)
        {
            var paging = new Paging();
            paging.CurrentPage = (offset / limit) + 1;
            paging.PageSize = limit;

            return new
            {
                rows = UALiteDb.GetModels(paging, groupId),
                total = paging.Count
            };
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public object UA(int id)
        {
            return UALiteDb.Get(id);
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public bool UpdateUA(UAModel ua)
        {
            UALiteDb.AddOrUpdate(ua);
            return true;
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        public bool RemoveUAs(string ids)
        {
            var removes = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(m => Convert.ToInt32(m)).ToArray();
            return UALiteDb.Remove(removes);
        }

        #endregion
        [HttpGet]
        public bool IsAlone()
        {
            return NodeConfigurationSection.Alone;
        }
    }

    public class ComplieFuncTest : Core.Compile.UrlCompile
    {
        private string code;

        public ComplieFuncTest(string code)
        {
            this.code = code;
        }

        public override string FormatCode(ExtractFunctionResult result)
        {
            var formatCode = string.Format(code, result.Args);

            return formatCode;
        }
    }
}
