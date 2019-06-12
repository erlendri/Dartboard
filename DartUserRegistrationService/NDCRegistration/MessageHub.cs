using Dart.Messaging;
using Dart.Messaging.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using NDCRegistration.MessageHubModels;
using NDCRegistration.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NDCRegistration.Hubs
{
    public class MessageHub : Hub
    {
        public Guid Id { get; }
        private readonly IMqttHandler _mqttHandler;
        private readonly IGamerContextMethods _gamerStorage;
        private readonly IHubContext<MessageHub> _hubContext;

        public string ApiUri { get; }

        public MessageHub(IMqttHandler mqttHandler, IGamerContextMethods gamerStorage, IHubContext<MessageHub> context, IConfiguration configuration)
        {
            Id = Guid.NewGuid();
            _mqttHandler = mqttHandler;
            _gamerStorage = gamerStorage;
            _hubContext = context;
            ApiUri = configuration["ApiUri"];

        }
        public async Task StartGame(Guid id)
        {
            var gamer = _gamerStorage.GetGamer(id).ToMinimal();
            await Task.Run(() =>
            {
                _mqttHandler.PostGameStarted(gamer);
            });
        }
        public async Task DeleteGame(Guid id)
        {
            var game = _gamerStorage.GetGamerLastPendingGame(id);
            if (game != null)
                _gamerStorage.DeleteGame(game.Id);
            await Task.Run(() =>
            {
                _mqttHandler.SyncClientGames();
            });

        }
        public async Task GetPendingGames()
        {
            var gamers = _gamerStorage.GetGamers();
            await MessageHubMethods.SendAllPendingGames(_hubContext, gamers, _mqttHandler.GetCurrentGameAsSignalR);

        }
        public async Task GetCompletedGames()
        {
            var gamers = _gamerStorage.GetGamers();
            await MessageHubMethods.SendAllCompletedGames(_hubContext, gamers);
        }
        public async Task GetCurrentGame()
        {
            await MessageHubMethods.SendCurrentGame(_hubContext, _mqttHandler.GetCurrentGameAsSignalR);
        }
        public async Task LookupQr(string qr)
        {
            var gamers = _gamerStorage.GetGamers();
            var gamer = gamers.FirstOrDefault(f => f.QrCode == qr);
            if (gamer != null)
                await Clients.Caller.SendAsync(SignalRTopics.UserLookup, gamer);
        }
        public async Task TestThrowDart()
        {
            await Task.Run(() =>
            {
                var currentGame = _mqttHandler.GameToSignalR(_mqttHandler.CurrentGame, out Gamer gamer);
                if (currentGame == null)
                    return;
                _mqttHandler.PostString(Topics.DartboardTest, "20;1");
            });

        }
        public async Task TestUpdateScore()
        {
            await Task.Run(() =>
            {
                var currentGame = _mqttHandler.GameToSignalR(_mqttHandler.CurrentGame, out Gamer gamer);
                if (currentGame == null)
                    return;
                var gamerMini = gamer.ToMinimal();
                gamerMini.Score = new Random().Next(10) + currentGame.Score;
                gamerMini.MaxTries = 3;
                gamerMini.Tries = _mqttHandler.CurrentGame.Tries + 1;
                _mqttHandler.PostCustom(Topics.ScoreUpdate, gamerMini);
            });

        }
        public async Task QueueRandomPlayer()
        {
            await Task.Run(() =>
            {
                var gamers = _gamerStorage.GetGamers();
                var gamer = new Gamer
                {
                    DisplayName = $"Gamer {gamers.Count + 1}",
                    QrCode = Guid.NewGuid().ToString()
                };
                _gamerStorage.CreateOrUpdateGamer(gamer);
                var game = _gamerStorage.CreateGame(gamer.Id);
                _mqttHandler.SyncClientGames();
            });
        }
    }
}