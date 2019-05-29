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
        internal async static Task SendGameAdded(IHubContext<MessageHub> hubContext, Gamer gamer)
        {
            var signalRGame = new SignalRGame(gamer);
            await hubContext.Clients.All.SendAsync(SignalRTopics.GameAdded, signalRGame);
        }
        internal async static Task SendGameUpdated(IHubContext<MessageHub> hubContext, Guid gamerId, int score)
        {
            var signalRGame = new SignalRGame(gamerId, score);
            await hubContext.Clients.All.SendAsync(SignalRTopics.GameUpdated, signalRGame);
        }
        internal async static Task SendGameDeleted(IHubContext<MessageHub> hubContext, Guid gamerId)
        {
            await hubContext.Clients.All.SendAsync(SignalRTopics.GameDeleted, gamerId);
        }
        internal async static Task SendGameCompleted(IHubContext<MessageHub> hubContext, Guid gamerId, int score)
        {
            var signalRGame = new SignalRGame(gamerId, score);
            await hubContext.Clients.All.SendAsync(SignalRTopics.GameCompleted, signalRGame);
        }
        internal async static Task SendAllPendingGames(IHubContext<MessageHub> hubContext, List<Gamer> gamers)
        {
            var games = gamers
                .Where(f => f.Games.Any(st => st.State == GameState.Pending))
                .Select(f => new SignalRGame(f)).ToList();

            await hubContext.Clients.All.SendAsync(SignalRTopics.GamesPending, games);
        }
        internal async static Task SendAllCompletedGames(IHubContext<MessageHub> hubContext, List<Gamer> gamers)
        {
            var games = gamers
                .Where(f => f.Games.Any(st => st.State == GameState.Completed))
                .Select(f =>
                {
                    var game = f.Games
                    .OrderByDescending(g => g.Score).First();
                    return new SignalRGame(f, game.Score);
                })
                .OrderByDescending(f=>f.Score).Take(30)
                .ToList();

            await hubContext.Clients.All.SendAsync(SignalRTopics.GamesCompleted, games);
        }
    }
}
