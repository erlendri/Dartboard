using System;
using System.Collections.Generic;

namespace Dart.GameManager.Models
{
    public class Gamer
    {
        public Gamer()
        {
            Id = Guid.NewGuid();
            Games = new List<Game>();
        }

        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Game> Games { get; set; }
        public Guid? BestGameId { get; set; }
        public string QrData { get; set; }
    }
}