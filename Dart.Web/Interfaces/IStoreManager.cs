using System;
using System.Collections.Generic;
using Dart.GameManager.Models;

namespace Dart.Web.Interfaces
{
    public interface IStoreManager
    {
        event EventHandler<Guid> GamerUpdated;

        List<Gamer> GetGamers();
        Gamer AddOrUpdateGamer(Gamer gamer, bool suppressUpdateEvent = false);
        Game AddOrUpdateGame(Game game);

        void DeleteGame(Guid gameId);
    }
}