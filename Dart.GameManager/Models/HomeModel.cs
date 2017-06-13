using System.Collections.Generic;

namespace Dart.GameManager.Models
{
    public class HomeModel
    {
        public List<Gamer> Gamers { get; set; }
        public Gamer CurrentGamer { get; set; }
    }
}