using System;
using System.Collections.Generic;
using System.Text;

namespace Dart.Messaging
{
    public class Score
    {
        public Score(Guid gamerId, int gameScore)
        {
            GamerId = gamerId;
            GameScore = gameScore;

        }
        public Guid GamerId { get; set; }
        public int GameScore { get; set; }
    }
}
