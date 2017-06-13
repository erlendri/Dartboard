using Dart.Web.Interfaces;
using System.Linq;
using System.Web.Mvc;
using Dart.GameManager.Models;

namespace Dart.Web.Controllers
{
    public class HomeController : Controller
    {
        private IStoreManager _storeManager;
        public HomeController(IStoreManager storeManager)
        {
            _storeManager = storeManager;
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View(CreateModel());
        }

        private HomeModel CreateModel()
        {
            var gamers = _storeManager.GetGamers();
            var currentGame = gamers.SelectMany(f => f.Games).FirstOrDefault(g => g.IsCurrent);
            var model = new HomeModel()
            {
                Gamers = gamers.Where(f=>f.Games.Any()).ToList()
            };
            if (currentGame != null)
                model.CurrentGamer = gamers.First(g=>g.Id == currentGame.GamerId);
            return model;

        }

    }
}
