using Dart.Messaging.Models;
using System;

namespace NDCRegistration.MessageHubModels
{
    public class SignalRGame
    {
        public SignalRGame(Guid gamerId, int score = 0)
        {

        }
        public SignalRGame(Gamer gamer, int score = 0) : this(gamer.Id, score)
        {
            Name = gamer.DisplayName;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

    }
}
