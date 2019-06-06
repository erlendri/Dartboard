using Dart.Messaging.Models;
using System;
using System.Collections.Generic;

namespace NDCRegistration
{
    public interface IGamerContextMethods
    {
        Gamer GetGamer(Guid id);
        Gamer CreateOrUpdateGamer(Gamer gamer);

        List<Gamer> GetGamers();
        Game CreateGame(Guid gamerId);
        void CompleteGame(Game game);
        void DeleteGame(Guid id);
        void UpdateGameScore(Guid id, int score);
    }
}
