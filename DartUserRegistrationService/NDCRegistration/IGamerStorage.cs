using Dart.Messaging.Models;
using System;
using System.Collections.Generic;

namespace NDCRegistration
{
    public interface IGamerStorage
    {
        Gamer GetGamer(Guid id);
        Gamer CreateOrUpdateGamer(Gamer gamer);

        List<Gamer> GetGamers();
        Game CreateGame(Game game);
        void CompleteGame(Game game);
        void DeleteGame(Guid id);

    }
}
