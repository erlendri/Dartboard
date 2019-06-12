using System.Diagnostics;
using Dart.Messaging.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NDCRegistration.Models;

namespace NDCRegistration.Controllers
{
    [Authorize]
    public class RegistrationController : Controller
    {
        private readonly IMqttHandler _handler;
        private readonly IGamerContextMethods _dbcontextMethods;

        public RegistrationController(IGamerContextMethods gamerStorage, IMqttHandler handler)
        {
            _handler = handler;
            _dbcontextMethods = gamerStorage;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Gamer model)
        {
            var gamer = _dbcontextMethods.CreateOrUpdateGamer(model);
            var game = _dbcontextMethods.CreateGame(gamer.Id);
            _handler.SyncClientGames();
            //post mqtt
            return View("index", new Gamer());
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
