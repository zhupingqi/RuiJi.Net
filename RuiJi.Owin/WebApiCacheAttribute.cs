using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace RuiJi.Owin
{
    public class WebApiCacheAttribute : ActionFilterAttribute
    {
        public int Duration { get; set; }

        public WebApiCacheAttribute()
        {
            Duration = 1;
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromSeconds(Duration),
                MustRevalidate = true,
                Public = true
            };
        }
    }
}
