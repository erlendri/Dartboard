using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dart.Messaging.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NDCRegistration
{
    public class GamerContextMethods : IGamerContextMethods
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private ILogger _logger;

        public GamerContextMethods(IServiceScopeFactory scopeFactory, ILogger<GamerContextMethods> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }


        public void CompleteGame(Game game)
        {
            UpdateGameState(game.Id, GameState.Completed, game.Score);
        }


        public Game CreateGame(Guid gamerId)
        {
            Game game = null;

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    var gamer = dbContext.Gamers
                        .First(f => f.Id == gamerId);
                    game = new Game
                    {
                        Score = 0,
                        State = GameState.Pending,
                        DateCreated = DateTime.Now
                    };
                    gamer.Games.Add(game);
                    dbContext.SaveChanges();
                    _logger.LogInformation($"Game created: {game.GamerId}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                }
            }
            return game;
        }


        public Gamer CreateOrUpdateGamer(Gamer gamer)
        {
            Gamer modifiedGamer = null;

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    var existing = dbContext.Gamers
                        .Include(f => f.Games)
                        .FirstOrDefault(f => !string.IsNullOrWhiteSpace(gamer.QrCode) && f.QrCode == gamer.QrCode);

                    if (existing != null)
                    {
                        var existingGames = existing.Games
                            .Where(f => f.State == GameState.Pending)
                            .ToList();

                        existingGames.Where(f => f.Score > 0).ToList()
                        .ForEach(f => f.State = GameState.Completed);
                        existingGames.Where(f => f.Score <= 0).ToList()
                        .ForEach(f => f.State = GameState.Deleted);

                        UpdateGamerEntity(existing, gamer);
                        gamer.Id = existing.Id;
                        modifiedGamer = gamer;
                    }
                    else
                    {
                        modifiedGamer = dbContext.Gamers.Add(gamer).Entity;
                    }
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                }
            }
            _logger.LogInformation($"Gamer created or modified: {modifiedGamer.DisplayName}");
            return modifiedGamer;
        }


        private void UpdateGamerEntity(Gamer existing, Gamer updated)
        {
            existing.DisplayName = updated.DisplayName;
            existing.QrCode = updated.QrCode;
        }


        public void DeleteGame(Guid id)
        {
            UpdateGameState(id, GameState.Deleted);
        }


        private void UpdateGameState(Guid gameId, GameState state, int? score = null)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    var game = dbContext.Gamers
                        .Include(f => f.Games)
                        .SelectMany(f => f.Games)
                        .FirstOrDefault(f => f.Id == gameId);
                    if (game != null)
                    {
                        game.State = state;
                        if (score.HasValue)
                            game.Score = score.Value;
                        dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                }
            }
        }


        public Gamer GetGamer(Guid id)
        {
            Gamer gamer = null;

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {

                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    gamer =
                        dbContext.Gamers
                        .Include(f => f.Games)
                        .First(f => f.Id == id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                }
            }

            return gamer;
        }


        public List<Gamer> GetGamers()
        {
            List<Gamer> gamers = null;
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {

                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    var gamersTest = dbContext.Gamers.ToList();
                    gamers =
                        dbContext.Gamers
                        .Include(f => f.Games)
                        .Where(f => f.Games.Any())
                        .ToList();

                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                }
            }
            return gamers;
        }


        public void UpdateGameScore(Guid gameId, int score)
        {
            UpdateGameState(gameId, GameState.Pending, score);
        }

        public Game GetGamerLastPendingGame(Guid gamerId)
        {
            Game game = null;

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    game =
                         dbContext.Games
                         .Where(f => f.GamerId == gamerId)
                         .Where(f => f.State == GameState.Pending)
                         .OrderByDescending(f => f.DateCreated)
                         .FirstOrDefault();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                }
            }
            return game;
        }
    }
}
