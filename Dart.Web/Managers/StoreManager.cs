using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.IO;
using Dart.GameManager.Models;
using Dart.Web.Interfaces;

namespace Dart.Web.Managers
{

    public class StoreManager : IStoreManager
    {
        public event EventHandler<Guid> GamerUpdated;
        private string _contactDir;

        public StoreManager()
        {
            if (_gamers == null)
                _gamers = new List<Gamer>();
            _contactDir = ConfigurationManager.AppSettings["contactDir"];
            if (!Directory.Exists(_contactDir))
                throw new ConfigurationErrorsException("File directory for storing contacts has NOT been set");
            LoadGamers();
        }

        private void LoadGamers()
        {
            var di = new DirectoryInfo(_contactDir);
            var fis = di.GetFiles("*.xml").ToList();
            if (fis.Count == 0)
                return;
            var ser = new ScoreSerializer<Gamer>();
            foreach (var fileInfo in fis)
            {
                var gamer = ser.DeSerializeObject(fileInfo.FullName);
                if (gamer.Games.Any())
                {
                    gamer.Games.All(f => f.IsCurrent = false);
                    gamer.BestGameId = gamer.Games.OrderByDescending(g => g.TotalScore).First().Id;
                }
                _gamers.RemoveAll(f => f.Id.Equals(gamer.Id));
                _gamers.Add(gamer);
            }
        }

        private void SaveAndFireEvent(Guid gamerId, bool suppressUpdateEvent = false)
        {
            //Ensure serialized to disk - shouldn't take more than a jiffy
            var existing = _gamers.First(f => f.Id.Equals(gamerId));
            var ser = new ScoreSerializer<Gamer>();
            ser.SerializeObject(existing, Path.Combine(_contactDir, $"{gamerId}.xml"));
            if (!suppressUpdateEvent)
                GamerUpdated?.Invoke(this, gamerId);

        }
        private static List<Gamer> _gamers = null;
        public List<Gamer> GetGamers()
        {
            return _gamers.OrderByDescending(f => f.Games.Count == 0 ? 0 : f.Games.Max(g => g.TotalScore)).ToList();
        }

        public Gamer AddOrUpdateGamer(Gamer gamer, bool suppressUpdateEvent = false)
        {
            Gamer existing;
            if (!string.IsNullOrWhiteSpace(gamer.QrData))
                existing = _gamers.FirstOrDefault(f => gamer.QrData.Equals(f.QrData));
            else
                existing = _gamers.FirstOrDefault(f => f.Id.Equals(gamer.Id));
            if (existing != null)
            {
                existing.DisplayName = gamer.DisplayName;
                if (gamer.Games.Any())
                {
                    existing.Games.RemoveAll(f => gamer.Games.Exists(g => g.Id == f.Id));
                    existing.Games.AddRange(gamer.Games);
                }
            }
            else
            {
                existing = gamer;
                _gamers.Add(gamer);
            }
            SaveAndFireEvent(existing.Id);
            return existing;
        }

        public Game AddOrUpdateGame(Game game)
        {
            var gamer = _gamers.First(g => g.Id.Equals(game.GamerId));
            var allGames = _gamers.SelectMany(f => f.Games).ToList();
            var toChangeCurrent = allGames.Where(f => f.IsCurrent).ToList();
            foreach (var gameCurrent in toChangeCurrent)
            {
                gameCurrent.IsCurrent = false;
                SaveAndFireEvent(gameCurrent.GamerId, true);
            }
            
            var existing = allGames.FirstOrDefault(g => g.Id.Equals(game.Id));
            if (existing != null)
            {
                existing.ScoreThrow1 = game.ScoreThrow1;
                existing.ScoreThrow2 = game.ScoreThrow2;
                existing.ScoreThrow3 = game.ScoreThrow3;
            }
            else
            {
                gamer.Games.Add(game);
                game.SequenceNo = gamer.Games.Max(f => f.SequenceNo) + 1;
                existing = game;
            }
            var bestGame = gamer.Games.OrderByDescending(g => g.TotalScore).First();
            gamer.BestGameId = bestGame.Id;
            existing.IsCurrent = true;
            SaveAndFireEvent(gamer.Id);
            return existing;
        }

        public void DeleteGame(Guid gameId)
        {
            var existing = _gamers.SelectMany(f => f.Games).First(g => g.Id.Equals(gameId));
            var gamer = _gamers.First(g => g.Id.Equals(existing.GamerId));
            gamer.Games.Remove(existing);
            SaveAndFireEvent(gamer.Id);
        }


    }
}