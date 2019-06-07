using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dart.Messaging.Models;
using Microsoft.AspNetCore.SignalR;
using NDCRegistration.Hubs;
using NDCRegistration.MessageHubModels;

namespace NDCRegistration
{
    public static class MessageHubMethods
    {
        internal async static Task SendGameUpdated(IHubContext<MessageHub> hubContext, Gamer gamer, int score)
        {
            var signalRGame = new SignalRGame(gamer.Id, gamer.DisplayName, score);
            await hubContext.Clients.All.SendAsync(SignalRTopics.ScoreUpdate, signalRGame);
        }
        internal async static Task SendAllPendingGames(IHubContext<MessageHub> hubContext, List<Gamer> gamers, SignalRGame currentGame)
        {
            var games = gamers
                .Where(f => f.Games.Any(st => st.State == GameState.Pending))
                .Where(f => currentGame == null || currentGame.Id != f.Id)
                .OrderBy(f=>f.Games.Where(g=>g.State == GameState.Pending).Max(g=>g.DateCreated))
                .Select(f => new SignalRGame(f.Id, f.DisplayName)).ToList();

            await hubContext.Clients.All.SendAsync(SignalRTopics.GamesPending, games);
        }

        internal async static Task SendCurrentGame(IHubContext<MessageHub> hubContext, SignalRGame currentGame)
        {
            await hubContext.Clients.All.SendAsync(SignalRTopics.GameCurrent, currentGame);
        }

        internal async static Task SendAllCompletedGames(IHubContext<MessageHub> hubContext, List<Gamer> gamers)
        {
            var games = gamers
                .Where(f => f.Games.Any(st => st.State == GameState.Completed))
                .Select(f =>
                {
                    var game = f.Games
                    .OrderByDescending(g => g.Score).First();
                    return new SignalRGame(f.Id, f.DisplayName, game.Score);
                })
                .OrderByDescending(f => f.Score).Take(30)
                .ToList();

            await hubContext.Clients.All.SendAsync(SignalRTopics.GamesCompleted, games);
        }
    }
}
