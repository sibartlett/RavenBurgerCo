using System;
using System.Web.Mvc;

namespace RavenBurgerCo.Controllers
{
    public class HomeController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var builder = new UriBuilder(Request.Url);
            builder.Port = MvcApplication.DocumentStore.HttpServer.Configuration.Port;
            ViewBag.RavenStudioUrl = builder.Uri.ToString();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Map()
        {
            return View();
        }

        public ActionResult Eatin()
        {
            return View();
        }

        public ActionResult Delivery()
        {
            return View();
        }

        public ActionResult DriveThru()
        {
            return View();
        }
    }
}
