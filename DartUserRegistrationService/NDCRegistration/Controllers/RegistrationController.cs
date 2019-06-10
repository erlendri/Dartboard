using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dart.Messaging.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NDCRegistration.Hubs;
using NDCRegistration.Models;

namespace NDCRegistration.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IHubContext<MessageHub> _hubContext;
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
            var hasQr = !string.IsNullOrWhiteSpace(model.QrCode);
            var hasDetails =
                !string.IsNullOrWhiteSpace(model.FirstName) &&
                !string.IsNullOrWhiteSpace(model.LastName) &&
                !string.IsNullOrWhiteSpace(model.Email);
            var hasDetailsAny =
                !string.IsNullOrWhiteSpace(model.FirstName) ||
                !string.IsNullOrWhiteSpace(model.LastName) ||
                !string.IsNullOrWhiteSpace(model.Email);
            if (hasQr || hasDetails)
            {
                var gamer = _dbcontextMethods.CreateOrUpdateGamer(model);
                var game = _dbcontextMethods.CreateGame(gamer.Id);
                _handler.SyncClientGames();
                //post mqtt
                Response.Redirect("index");
            }
            else if (hasDetailsAny)
            {
                if (string.IsNullOrWhiteSpace(model.FirstName))
                    ModelState.AddModelError(nameof(model.FirstName), "Enter first name");
                if (string.IsNullOrWhiteSpace(model.LastName))
                    ModelState.AddModelError(nameof(model.LastName), "Enter last name");
                if (string.IsNullOrWhiteSpace(model.Email))
                    ModelState.AddModelError(nameof(model.Email), "Enter email");
            }
            else
            {
                ModelState.AddModelError("", "Either scan a QR code, or enter the name + details below");
            }
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
