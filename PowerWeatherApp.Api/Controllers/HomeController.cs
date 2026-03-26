using System.Web.Mvc;

namespace PowerWeatherApp.Api.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}