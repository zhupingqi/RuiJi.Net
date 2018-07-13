using RuiJi.Net.Core.Compile;
using RuiJi.Net.Core.Configuration;
using RuiJi.Net.Core.Utils.Page;
using RuiJi.Net.Node;
using RuiJi.Net.Node.Feed.Db;
using System;
using System.Linq;
using System.Web.Http;

namespace RuiJi.Net.Owin.Controllers
{
    [RoutePrefix("api/setting")]
    public class SettingApiController : ApiController
    {
        #region 节点设置
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("nodes")]
        public object Nodes()
        {
            var leaderNode = ServerManager.GetLeader();
            if (leaderNode != null)
            {
                var feeds = leaderNode.GetChildren("/live_nodes/feed");
                var crawlers = leaderNode.GetChildren("/live_nodes/crawler");

                return new
                {
                    feeds,
                    crawlers
                };
            }

            return new object();
        }
        #endregion

        #region 节点函数
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("func/list")]
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
        [Route("func")]
        public object GetFunc(int id)
        {
            return FuncLiteDb.Get(id);
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("func/update")]
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
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("func/remove")]
        public bool RemoveFunc(string ids)
        {
            var removes = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(m => Convert.ToInt32(m)).ToArray();

            return FuncLiteDb.Remove(removes);
        }
        #endregion

        #region Proxys
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("proxy/list")]
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
        [Route("proxy/update")]
        public object UpdateProxy(ProxyModel proxy)
        {
            ProxyLiteDb.AddOrUpdate(proxy);

            return true;
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("proxy")]
        public object GetProxy(int id)
        {
            var feed = ProxyLiteDb.Get(id);

            return feed;
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("proxy/remove")]
        public bool RemoveProxy(string ids)
        {
            var removes = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(m => Convert.ToInt32(m)).ToArray();

            return ProxyLiteDb.Remove(removes);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("proxy/status")]
        public bool ProxyStatusChange(string ids, string status)
        {
            var changeIds = ids.Split(',').Select(i => Convert.ToInt32(i)).ToArray();
            var statusEnum = (Status)Enum.Parse(typeof(Status), status.ToUpper());

            return ProxyLiteDb.StatusChange(changeIds, statusEnum);
        }
        #endregion

        #region uaGroup
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("ua/group/list")]
        public object UAGroups()
        {
            return UAGroupLiteDb.GetModels();
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("ua/group")]
        public object UAGroup(int id)
        {
            return UAGroupLiteDb.Get(id);
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("ua/group/update")]
        public int UpdateUAGroup(UAGroupModel group)
        {
            return UAGroupLiteDb.AddOrUpdate(group);
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("ua/group/remove")]
        public bool RemoveUAGroup(int id)
        {
            return UALiteDb.RemoveByGorup(id) ? UAGroupLiteDb.Remove(id) : false;
        }
        #endregion

        #region ua
        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("ua/list")]
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
        [Route("ua")]
        public object UA(int id)
        {
            return UALiteDb.Get(id);
        }

        [HttpPost]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("ua/update")]
        public bool UpdateUA(UAModel ua)
        {
            UALiteDb.AddOrUpdate(ua);
            return true;
        }

        [HttpGet]
        [NodeRoute(Target = NodeTypeEnum.FEEDPROXY)]
        [Route("ua/remove")]
        public bool RemoveUAs(string ids)
        {
            var removes = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(m => Convert.ToInt32(m)).ToArray();
            return UALiteDb.Remove(removes);
        }

        #endregion
    }
}