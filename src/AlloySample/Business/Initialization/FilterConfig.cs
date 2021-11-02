using System;
using System.Collections.Concurrent;
using System.Web;
using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace AlloySample.Business.Initialization
{
    /// <summary>
    /// Module for registering filters which will be applied to controller actions.
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class FilterConfig : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            GlobalFilters.Filters.Add(ServiceLocator.Current.GetInstance<PageContextActionFilter>());
            GlobalFilters.Filters.Add(ServiceLocator.Current.GetInstance<AddCustomHeaderActionFilterAttribute>());
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }

    public class AddCustomHeaderActionFilterAttribute : ActionFilterAttribute
    {
        private static ConcurrentDictionary<string, DateTime> _serverTotalTime = new ConcurrentDictionary<string, DateTime>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Items["querytime"] = 0;

            var requestId = Guid.NewGuid();
            filterContext.HttpContext.Items["requestId"] = requestId.ToString();
            _serverTotalTime.AddOrUpdate(requestId.ToString(), DateTime.Now, (s, time) => time);

            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            filterContext.HttpContext.Response.Headers.Add("Server-Timing",
                "queryTime;desc=\"GetChildren Query\";dur=" + filterContext.HttpContext.Items["querytime"]);
            var dateTime = _serverTotalTime[filterContext.HttpContext.Items["requestId"].ToString()];
            filterContext.HttpContext.Response.Headers.Add("Server-Timing",
                "totalTime;desc=\"Total store execution\";dur=" + (DateTime.Now - dateTime).Milliseconds);
        }
    }
}
