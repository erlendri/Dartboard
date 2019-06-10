using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dart.Messaging.Models;
using Microsoft.EntityFrameworkCore;

namespace NDCRegistration
{
    public class LiveGamerStorage : IGamerContextMethods
    {
        private GamerContext gamerContext;

        public LiveGamerStorage(GamerContext activeGamerContext)
        {
            gamerContext = activeGamerContext;
        }
       
        public void CompleteGame(Game game)
        {
            var existing = gamerContext.Games.FirstOrDefault(it => it.Id == game.Id);
            if (existing != null)
            {
                existing.Score = game.Score;
                existing.State = GameState.Completed;
            }
            gamerContext.SaveChanges();
        }

        public Game CreateGame(Game game)
        {   
            game.State = GameState.Pending;
            var createdGame = gamerContext.Games.Add(game).Entity;
            gamerContext.SaveChanges();

            return createdGame;
        }

        public Game CreateGame(Guid gamerId)
        {
            throw new NotImplementedException();
        }

        public Gamer CreateOrUpdateGamer(Gamer gamer)
        {
            Gamer resultGame;

            var existing = gamerContext.Gamers.FirstOrDefault(f =>
            (!string.IsNullOrWhiteSpace(gamer.QrCode) && f.QrCode == gamer.QrCode) ||
            (!string.IsNullOrWhiteSpace(gamer.Email) && f.Email == gamer.Email));

            if (existing != null)
            {
                existing.DisplayName = gamer.DisplayName ?? "";
                existing.Email = gamer.Email ?? "";
                existing.FirstName = gamer.FirstName ?? "";
                existing.LastName = gamer.LastName ?? "";
                existing.QrCode = gamer.QrCode ?? "";
                resultGame = existing;
            }
            else
            {
               resultGame = gamerContext.Gamers.Add(gamer).Entity;
            }

            gamerContext.SaveChanges();

            return resultGame;
        }

        public void DeleteGame(Guid id)
        {
            var game = gamerContext.Games.First(f => f.Id == id);
            if (game != null)
                game.State = GameState.Deleted;

            gamerContext.SaveChanges();
        }

        public Gamer GetGamer(Guid id)
        {
            return gamerContext.Gamers.First(f => f.Id == id);
        }

        public Game GetGamerLastPendingGame(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Gamer> GetGamers()
        {
            return gamerContext.Gamers.ToList();
        }

        public void UpdateGameScore(Guid id, int score)
        {
            throw new NotImplementedException();
        }
    }
}
