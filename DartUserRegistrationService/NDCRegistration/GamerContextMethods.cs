using System;
using System.Collections.Generic;
using System.Linq;
using Dart.Messaging.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NDCRegistration
{
    public class GamerContextMethods : IGamerContextMethods
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public GamerContextMethods(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void CompleteGame(Game game)
        {
            UpdateGameState(game.Id, GameState.Completed, game.Score);
        }

        public Game CreateGame(Guid gamerId)
        {

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    var gamer = dbContext.Gamers
                        .First(f => f.Id == gamerId);
                    var game = new Game
                    {
                        Score = 0,
                        State = GameState.Pending,
                        DateCreated = DateTime.Now
                    };
                    gamer.Games.Add(game);
                    dbContext.SaveChanges();
                    return game;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public Gamer CreateOrUpdateGamer(Gamer gamer)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {

                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    var existing = dbContext.Gamers
                        .Include(f => f.Games)
                        .FirstOrDefault(f =>
            (!string.IsNullOrWhiteSpace(gamer.QrCode) && f.QrCode == gamer.QrCode) ||
            (!string.IsNullOrWhiteSpace(gamer.Email) && f.Email == gamer.Email));

                    if (existing != null)
                    {
                        var existingGames = existing.Games
                            .Where(f => f.State == GameState.Pending)
                            .ToList();

                        existingGames.Where(f=>f.Score > 0).ToList()
                        .ForEach(f => f.State = GameState.Completed);
                        existingGames.Where(f => f.Score <= 0).ToList()
                        .ForEach(f => f.State = GameState.Deleted);

                        UpdateGamerEntity(existing, gamer);
                        gamer.Id = existing.Id;
                    }
                    else
                    {
                        dbContext.Gamers.Add(gamer);
                    }
                    dbContext.SaveChanges();
                    return gamer;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }

        private void UpdateGamerEntity(Gamer existing, Gamer updated)
        {
            existing.DisplayName = updated.DisplayName;
            existing.Email = updated.Email;
            existing.FirstName = updated.FirstName;
            existing.LastName = updated.LastName;
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
                    throw;
                }
            }

        }

        public Gamer GetGamer(Guid id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {

                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    var gamer =
                        dbContext.Gamers
                        .Include(f => f.Games)
                        .First(f => f.Id == id);

                    return gamer;
                }
                catch (Exception ex)
                {
                    var msg = ex.ToString();
                    throw;
                }
            }
        }

        public List<Gamer> GetGamers()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {

                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    var gamersTest = dbContext.Gamers.ToList();
                    var gamers =
                        dbContext.Gamers
                        .Include(f => f.Games)
                        .Where(f => f.Games.Any())
                        .ToList();

                    return gamers;
                }
                catch (Exception ex)
                {
                    var msg = ex.ToString();
                    throw;
                }
            }
        }

        public void UpdateGameScore(Guid gameId, int score)
        {
            UpdateGameState(gameId, GameState.Pending, score);
        }

        public Game GetGamerLastPendingGame(Guid gamerId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {

                    var dbContext = scope.ServiceProvider.GetRequiredService<GamerContext>();
                    var game =
                        dbContext.Games
                        .Where(f => f.GamerId == gamerId)
                        .Where(f => f.State == GameState.Pending)
                        .OrderByDescending(f => f.DateCreated)
                        .FirstOrDefault();

                    return game;
                }
                catch (Exception ex)
                {
                    var msg = ex.ToString();
                    throw;
                }
            }
        }
    }
}
