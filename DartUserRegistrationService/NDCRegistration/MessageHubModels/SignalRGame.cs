using Dart.Messaging.Models;
using System;

namespace NDCRegistration.MessageHubModels
{
    public class SignalRGame
    {
        public SignalRGame(Guid gamerId, string name, int score = 0)
        {
            Id = gamerId;
            Name = name;
            Score = score;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

    }
}
