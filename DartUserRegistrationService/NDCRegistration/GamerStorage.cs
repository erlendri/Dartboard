using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dart.Messaging.Models;

namespace NDCRegistration
{
    public class GamerStorage : IGamerStorage
    {
        public static List<Gamer> Gamers { get; set; } = new List<Gamer>();
        public static List<Game> Games { get; set; } = new List<Game>();

        public void CompleteGame(Game game)
        {
            var existing = Games.FirstOrDefault(f => f.Id == game.Id);
            if (existing != null)
            {
                existing.Score = game.Score;
                existing.State = GameState.Completed;
            }
        }

        public Game CreateGame(Game game)
        {
            game.Id = Guid.NewGuid();
            game.State = GameState.Pending;
            Games.Add(game);
            return game;
        }

        public Gamer CreateOrUpdateGamer(Gamer gamer)
        {
            var existing = Gamers.FirstOrDefault(f => 
            (!string.IsNullOrWhiteSpace(gamer.QrCode) && f.QrCode == gamer.QrCode) || 
            (!string.IsNullOrWhiteSpace(gamer.Email) && f.Email == gamer.Email));
            if (existing != null)
            {
                Gamers.Remove(existing);
                gamer.Id = Guid.NewGuid();
                Gamers.Add(gamer);
            }
            return gamer;
        }

        public void DeleteGame(Guid id)
        {
            var game = Games.First(f => f.Id == id);
            if (game != null)
                game.State = GameState.Deleted;
        }

        public Gamer GetGamer(Guid id)
        {
            return Gamers.First(f => f.Id == id);
        }

        public List<Gamer> GetGamers()
        {
            return Gamers;
        }
    }
}
