using System.Web.Mvc;
using Dart.GameManager.Models;


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
