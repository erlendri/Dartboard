using Dart.Messaging;
using Dart.Messaging.Models;
using Microsoft.AspNetCore.SignalR;
using NDCRegistration.MessageHubModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NDCRegistration.Hubs
{
    public class MessageHub : Hub
    {
        public Guid Id { get; }
        private readonly IMqttHandler _mqttHandler;
        private readonly IGamerContextMethods _gamerStorage;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageHub(IMqttHandler mqttHandler, IGamerContextMethods gamerStorage, IHubContext<MessageHub> context)
        {
            Id = Guid.NewGuid();
            _mqttHandler = mqttHandler;
            _gamerStorage = gamerStorage;
            _hubContext = context;

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
        public async Task TestUpdateCurrent()
        {
            await Task.Run(() =>
            {
                var currentGame = _mqttHandler.GameToSignalR(_mqttHandler.CurrentGame, out Gamer gamer);
                if (currentGame == null)
                    return;
                var gamerMini = gamer.ToMinimal();
                gamerMini.Score = new Random().Next(100) + currentGame.Score;
                _mqttHandler.PostCustom(Topics.ScoreUpdate, gamerMini);
            });

        }
        public async Task TestCompleteGame()
        {
            await Task.Run(() =>
            {
                var currentGame = _mqttHandler.GameToSignalR(_mqttHandler.CurrentGame, out Gamer gamer);
                if (currentGame == null)
                    return;
                var gamerMini = gamer.ToMinimal();
                gamerMini.Score = new Random().Next(100) + currentGame.Score;
                gamerMini.MaxTries = 0;
                _mqttHandler.PostCustom(Topics.ScoreUpdate, gamerMini);
            });

        }
    }
}