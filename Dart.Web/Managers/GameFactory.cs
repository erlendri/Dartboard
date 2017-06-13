using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dart.GameManager.Models;

namespace Dart.Web.Hubs
{
    public class GameFactory
    {
        private static int _gamerId = 1;
        public static Gamer CreateDebugGamer()
        {
            var id = Guid.NewGuid();
            var gamer = new Gamer()
            {
                QrData = $"Gamer {_gamerId++}"
            };
            return gamer;
        }

        //public static Game CreateDebugGame(Guid gamerId, Random rng)
        //{
        //    return new Game()
        //    {
        //        Id = Guid.NewGuid(),
        //        StartTime = DateTime.Now.AddMinutes(-(rng.Next(300))),
        //        GamerId = gamerId,
        //        ScoreThrow1 = rng.Next(100),
        //        ScoreThrow2 = rng.Next(100),
        //        ScoreThrow3 = rng.Next(100)
        //    };
        //}
        public static Game CreateGame(Guid gamerId)
        {
            return new Game()
            {
                Id = Guid.NewGuid(),
                StartTime = DateTime.Now,
                GamerId = gamerId,
            };
        }


    }
}