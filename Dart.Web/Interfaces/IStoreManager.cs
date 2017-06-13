using Dart.Web.Models;
using System;
using System.Collections.Generic;

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