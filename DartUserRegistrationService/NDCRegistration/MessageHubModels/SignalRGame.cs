using Dart.Messaging.Models;
using System;
using System.Linq;

namespace NDCRegistration.MessageHubModels
{
    public class SignalRGame
    {
        public SignalRGame(Guid key, string name, Game game, int tries, int maxTries) : this(key, name, game.Score, tries, maxTries, game.DateCreated)
        {

        }

        public SignalRGame(Guid gamerId, string name, int score, int tries, int maxTries, DateTime? dateCreated = null)
        {
            Id = gamerId;
            Name = name;
            Score = score;
            Tries = tries;
            MaxTries = maxTries;
            DateCreated = dateCreated;
            Nonce = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int Tries { get; set; }
        public int MaxTries { get; set; }
        public DateTime? DateCreated { get; }
        public Guid Nonce { get; set; }
    }
}
