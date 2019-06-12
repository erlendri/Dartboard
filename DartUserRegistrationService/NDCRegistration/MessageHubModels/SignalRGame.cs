using Dart.Messaging.Models;
using System;

namespace NDCRegistration.MessageHubModels
{
    public class SignalRGame
    {
        public SignalRGame(Guid gamerId, string name, int score, int tries, int maxTries)
        {
            Id = gamerId;
            Name = name;
            Score = score;
            Tries = tries;
            MaxTries = maxTries;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Tries { get; set; }
        public int MaxTries { get; set; }
    }
}
