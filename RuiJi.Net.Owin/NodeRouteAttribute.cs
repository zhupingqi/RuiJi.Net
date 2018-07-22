using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using RestSharp;
using RuiJi.Net.Core.Extensions;
using RuiJi.Net.Node;
using RuiJi.Net.NodeVisitor;
using System.Threading;

namespace RuiJi.Net.Owin
{
    public class NodeRouteAttribute : ActionFilterAttribute
    {
        public NodeTypeEnum Target { get; set; }

        public string RouteArgumentName { get; set; }

        public NodeRouteAttribute()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var node = ServerManager.Get(actionContext.HttpContext.Request.Host.Value);

            if (node == null)
            {
                actionContext.Result = new JsonResult(actionContext.HttpContext.Request.Host.Value + " no node mapping this uri");
            }

            if ((int)node.NodeType != (int)Target && node.NodeType != NodeTypeEnum.STANDALONE)
            {
                var baseUrl = "";

                if ((int)Target < 3)
                {
                    baseUrl = ProxyManager.Instance.Elect((NodeProxyTypeEnum)Target);
                }
                else
                {
                    baseUrl = actionContext.ActionArguments[RouteArgumentName].ToString();
                }

                var client = new RestClient("http://" + baseUrl);
                var restRequest = new RestRequest(actionContext.HttpContext.Request.Path.Value + actionContext.HttpContext.Request.QueryString);
                restRequest.Method = (actionContext.HttpContext.Request.Method == "GET") ? Method.GET : Method.POST;
                restRequest.JsonSerializer = new NewtonJsonSerializer();
                if (restRequest.Method == Method.POST)
                {
                    foreach (var arg in actionContext.ActionArguments)
                    {
                        restRequest.AddJsonBody(arg.Value);
                    }
                }

                var resetEvent = new ManualResetEvent(false);

                var handle = client.ExecuteAsync(restRequest, (restResponse) =>
                {
                    var m = ((ControllerActionDescriptor)actionContext.ActionDescriptor).MethodInfo;

                    if (m.ReturnType != null)
                    {
                        var obj = JsonConvert.DeserializeObject<object>(restResponse.Content);

                        actionContext.Result = new JsonResult(obj);
                    }
                    else
                    {
                        actionContext.Result = null;
                    }

                    resetEvent.Set();
                });

                resetEvent.WaitOne();
            }
            else
            {
                base.OnActionExecuting(actionContext);
            }
        }
    }
}