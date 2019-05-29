using System;
using System.Collections.Generic;
using System.Text;

namespace Dart.Messaging.Models
{
    public enum GameState
    {
        Pending,
        Completed,
        Deleted
    }
    public class Game
    {
        public Guid GamerId { get; set; }
        public Guid Id { get; set; }

        public int Score { get; set; }
        public GameState State { get; set; }

    }
}
