using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dart.Messaging.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NDCRegistration.Hubs;
using NDCRegistration.Models;

namespace NDCRegistration.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IGamerContextMethods gamerContextMethods;

        public HomeController(IConfiguration configuration, IGamerContextMethods gamerContextMethods)
        {
            this.configuration = configuration;
            this.gamerContextMethods = gamerContextMethods;
        }
        public IActionResult Index()
        {
            return View();
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
        public IActionResult Highscore(string qr)
        {
            var uri = configuration.GetValue<string>("ApiUri");
            var gamerId = Guid.Parse(qr);
            var gamer = gamerContextMethods.GetGamer(gamerId);
            var userJson = new List<string>();
            try
            {
                using (var client = new HttpClient())
                {
                    if (!float.TryParse(gamer.QrCode, out float number))
                    {
                        userJson.Add($"Invalid QR: {gamer.QrCode}");
                    }
                    var userUri = $"{uri}{gamer.QrCode}";
                    //var userUri = $"{uri}{"9626442211223632793001"}";
                    var task = client.GetStringAsync(userUri);
                    task.Wait();
                    if (!task.IsCompletedSuccessfully)
                        throw new ApplicationException($"error on lookup. code: {task.Result}");
                    userJson.Add(task.Result);
                }
            }
            catch (Exception ex)
            {
                userJson.Add($"Lookup failed for qr {gamer.QrCode}");
            }


            var model = new HighscoreModel
            {
                UserJson = userJson
            };
            return View(model);
        }
    }
}
