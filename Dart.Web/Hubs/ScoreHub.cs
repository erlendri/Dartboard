using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dart.GameManager;
using Microsoft.AspNet.SignalR;
using Dart.Web.Interfaces;
using Dart.Web.Models;

namespace Dart.Web.Hubs
{
    public class ScoreHub : Hub
    {
        private readonly IStoreManager _storeManager;
        private Random rng;
        private IGameManager _gameManager;

        public ScoreHub(IStoreManager storeManager, IGameManager gameManager)
        {
            _storeManager = storeManager;
            _gameManager = gameManager;
            _gameManager.GameScoreUpdated += GameManagerGameScoreUpdated;
            _storeManager.GamerUpdated += StoreManagerGamerUpdated;
            rng = new Random();

        }

        private void GameManagerGameScoreUpdated(object sender, OnScoreUpdatedArgs args)
        {
            var currentGame = GetCurrentGame();
            if (currentGame != null)
            {
                currentGame.ScoreThrow1 = args.CurrentGame.FirstThrow == 0 ? (int?)null : args.CurrentGame.FirstThrow;
                currentGame.ScoreThrow2 = args.CurrentGame.SecondThrow == 0 ? (int?)null : args.CurrentGame.SecondThrow;
                currentGame.ScoreThrow3 = args.CurrentGame.ThirdThrow == 0 ? (int?)null : args.CurrentGame.ThirdThrow;
                _storeManager.AddOrUpdateGame(currentGame);
            }
        }

        private void StoreManagerGamerUpdated(object sender, Guid e)
        {
            Clients.All.refresh();
        }

        public void RefreshScore(Guid personId)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.refreshScore(personId);
        }

        private Game GetCurrentGame()
        {
            var game = _storeManager.GetGamers().SelectMany(g => g.Games).FirstOrDefault(f => f.IsCurrent);
            return game;

        }
        public void SimulateThrow()
        {
            var game = GetCurrentGame();
            if (game != null)
            {
                if (game.ScoreThrow1 == null)
                    game.ScoreThrow1 = rng.Next(100);
                else if (game.ScoreThrow2 == null)
                    game.ScoreThrow2 = rng.Next(100);
                else if (game.ScoreThrow3 == null)
                    game.ScoreThrow3 = rng.Next(100);
                else
                    return;
                _storeManager.AddOrUpdateGame(game);
            }
            else
            {
                return;
            }
            // Call the addNewMessageToPage method to update clients.
        }

        public void DeleteGame(Guid gameId)
        {
            _storeManager.DeleteGame(gameId);
        }

        public Gamer AddPlayerFromQr(string qrData)
        {
            var gamer = _storeManager.GetGamers().FirstOrDefault(g => string.Equals(g.QrData, qrData, StringComparison.InvariantCultureIgnoreCase));
            if (gamer != null)
            {
                StartNewGame(gamer);
                return gamer;
            }
            gamer = new Gamer()
            {
                QrData = qrData
            };
            return gamer;
        }

        private void StartNewGame(Gamer gamer)
        {
            _gameManager.StartNewGame();
            _storeManager.AddOrUpdateGame(GameFactory.CreateGame(gamer.Id));
        }

        public Gamer AddPlayer(Gamer gamer)
        {
            var existing = _storeManager.GetGamers().FirstOrDefault(g => g.QrData.Equals(gamer.QrData));
            if (existing == null)
            {
                existing = _storeManager.AddOrUpdateGamer(gamer, true);
            }
            StartNewGame(existing);
            return gamer;
        }

    }
}