using System;
using System.Collections.Generic;
using System.Text;

namespace Dart.Messaging
{
    public static class Topics
    {
        public const string Gamer = "/geopackman/Gamer";
        public const string Score = "/geopackman/Score";

        ///// <summary>
        /// "{\"id\": \"007\", \"name\": \"James Bond\", \"maxTries\":3}"
        /// </summary>
        public const string GameStarted = "hackathon/player";

        //{"id":"007","name":"James Bond","maxTries":3,"score":16}
        public const string ScoreUpdate = "hackathon/player/score";

        public const string GameCompleted = "hackathon/player/completed";
        public const string GameAborted = "hackathon/player/aborted";

        public static Guid TestId = new Guid("7EC89CCF-E51C-49FB-8FFD-8DDC746A40BE");

    }
}
