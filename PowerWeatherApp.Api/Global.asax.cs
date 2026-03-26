using PowerWeatherApp.Api.App_Start;
using Serilog;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace PowerWeatherApp.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Trace(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: HttpContext.Current.Server.MapPath("~/App_Data/logs/log-.txt"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

            SwaggerConfig.Register();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            AutofacConfig.RegisterDependencies();
        }
    }
}
