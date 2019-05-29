using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDCRegistration
{
    public class SignalRTopics
    {
        public const string GameAdded = "GameAdded";
        public const string ScoreUpdate = "ScoreUpdate";
        public const string GameDeleted = "GameDeleted";
        public const string GameCompleted = "GameCompleted";
    }
}
