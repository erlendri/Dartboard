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
        internal async static Task SendAllPendingGames(IHubContext<MessageHub> hubContext, List<Gamer> gamers, SignalRGame currentGame)
        {
            var games = gamers
                .Where(f => f.Games.Any(st => st.State == GameState.Pending))
                .Where(f => currentGame == null || currentGame.Id != f.Id)
                .OrderBy(f=>f.Games.Where(g=>g.State == GameState.Pending).Max(g=>g.DateCreated))
                .Select(f => new SignalRGame(f.Id, f.DisplayName, 0, 0, 3)).ToList();

            await hubContext.Clients.All.SendAsync(SignalRTopics.GamesPending, games);
        }

        internal async static Task SendCurrentGame(IHubContext<MessageHub> hubContext, SignalRGame currentGame)
        {
            await hubContext.Clients.All.SendAsync(SignalRTopics.GameCurrent, currentGame);
        }

        internal async static Task SendAllCompletedGames(IHubContext<MessageHub> hubContext, List<Gamer> gamers)
        {
            List<SignalRGame> games = FilterTopCompletedGames(gamers);

            await hubContext.Clients.All.SendAsync(SignalRTopics.GamesCompleted, games);
        }

        public static List<SignalRGame> FilterTopCompletedGames(List<Gamer> gamers)
        {
            return gamers
                .Where(f => f.Games.Any(st => st.State == GameState.Completed))
                .GroupBy(f => f.Id)
                .Select(f => new SignalRGame(f.Key, f.Max(g => g.DisplayName), f.SelectMany(g => g.Games)
                .Where(h => h.State == GameState.Completed).OrderByDescending(g => g.Score).ThenBy(g => g.DateCreated).First(), 3, 3))
                .OrderByDescending(f => f.Score).ThenBy(f => f.DateCreated).Take(30)
                .ToList();
        }
        public static List<QrScoreModel> FilterTopCompletedGamer(List<Gamer> gamers)
        {
            return gamers
                .Where(f => f.Games.Any(st => st.State == GameState.Completed))
                .GroupBy(f => f.Id)
                .Select(f => new QrScoreModel(f.First(), f.SelectMany(g => g.Games)
                .Where(h => h.State == GameState.Completed).OrderByDescending(g => g.Score).ThenBy(g => g.DateCreated).First()))
                .OrderByDescending(f => f.Score).ThenBy(f => f.DateCreated).Take(30)
                .ToList();
        }

    }
}
