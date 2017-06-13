using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dart.Web.Models
{
    public class HomeModel
    {
        public List<Gamer> Gamers { get; set; }
        public Gamer CurrentGamer { get; set; }
    }
}