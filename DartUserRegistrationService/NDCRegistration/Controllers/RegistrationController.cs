using System.Diagnostics;
using Dart.Messaging.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NDCRegistration.Models;

namespace NDCRegistration.Controllers
{
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
            _handler.SyncClientGames();
            //post mqtt
            Response.Redirect("index");
            return View("index", model);
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
