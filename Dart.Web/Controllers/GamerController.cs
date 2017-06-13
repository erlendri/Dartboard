using Dart.Web.Models;
using System.Web.Mvc;


namespace Dart.Web.Controllers
{
    public class GamerController : Controller
    {
        public ActionResult Index()
        {
            return View(new HomeModel());
        }

    }
}
