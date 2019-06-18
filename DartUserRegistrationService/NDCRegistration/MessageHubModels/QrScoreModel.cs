using System;
using Dart.Messaging.Models;

namespace NDCRegistration.MessageHubModels
{
    public class QrScoreModel
    {
        public QrScoreModel(Gamer gamer, Game game)
        {
            QrCode = gamer.QrCode;
            Score = game.Score;
            DateCreated = game.DateCreated;
        }

        public string QrCode { get; set; }
        public int Score { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
