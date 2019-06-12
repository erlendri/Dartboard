using Dart.Messaging;
using Dart.Messaging.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NDCRegistration.Hubs;
using NDCRegistration.MessageHubModels;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace NDCRegistration
{
    public interface IMqttHandler
    {
        Guid Id { get; set; }
        void PostGameStarted(GamerMinimal gamer);
        Game CurrentGame { get; }
        SignalRGame GetCurrentGameAsSignalR { get; }
        void SyncClientGames();
        void PostCustom(string topic, object obj);
        void PostString(string topic, string text);
        SignalRGame GameToSignalR(Game currentGame, out Gamer gamer);
    }
    public class MqttHandler : IMqttHandler
    {
        public MqttHandler(IConfiguration config, IHubContext<MessageHub> hubContext, IGamerContextMethods gamerStorage)
        {
            Id = Guid.NewGuid();
            _config = config;
            _hubContext = hubContext;
            _gamerStorage = gamerStorage;
            var key = _config.GetValue<string>("MqttSettings:BrokerUri");
            _messageHandler = new MqttMessageHandler(key);
            _messageHandler.Subscribe(Topics.ScoreUpdate);
            _messageHandler.MqttMsgPublishReceived += _messageHandler_MqttMsgPublishReceived;
        }
        public void SyncClientGames()
        {
            var gamers = _gamerStorage.GetRelevantGamers(CurrentGame?.Id);
            if (CurrentGame != null)
            {
                var currentGame = gamers.SelectMany(f => f.Games).FirstOrDefault(g => g.Id == CurrentGame.Id);
                if (currentGame == null)
                    CurrentGame = null;
            }
            MessageHubMethods.SendCurrentGame(_hubContext, GetCurrentGameAsSignalR).Wait();
            MessageHubMethods.SendAllPendingGames(_hubContext, gamers, GetCurrentGameAsSignalR).Wait();
            MessageHubMethods.SendAllCompletedGames(_hubContext, gamers).Wait();
        }
        private void _messageHandler_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var message = Encoding.Default.GetString(e.Message);
            if (e.Topic == Topics.ScoreUpdate)
            {
                GetGamerFromMessage(message, out GamerMinimal gamerMinimal, out Gamer stored, out Game game);
                if (game == null)
                    return;
                _gamerStorage.UpdateGameScore(game.Id, gamerMinimal.Score);
                game.Score = gamerMinimal.Score;
                game.Tries = gamerMinimal.Tries;
                game.MaxTries = gamerMinimal.MaxTries;
                if (gamerMinimal.MaxTries <= gamerMinimal.Tries)
                {
                    _gamerStorage.CompleteGame(game);
                    game.State = GameState.Completed;
                }
                SetCurrentGame(game);
                SyncClientGames();
            }
        }

        private void SetCurrentGame(Game game)
        {
            if (CurrentGame != null && CurrentGame.Id != game.Id && CurrentGame.State == GameState.Pending)
            {
                _gamerStorage.DeleteGame(CurrentGame.Id);
            }
            if (game == null)
                CurrentGame = null;
            else
                CurrentGame = game;
        }

        private void GetGamerFromMessage(string message, out GamerMinimal gamer, out Gamer stored, out Game game)
        {
            gamer = JsonConvert.DeserializeObject<GamerMinimal>(message);
            stored = _gamerStorage.GetGamer(gamer.Id);
            game = stored.Games.OrderByDescending(g => g.DateCreated).First(g => g.State == GameState.Pending);
        }

        public void PostGameStarted(GamerMinimal gamer)
        {
            var game = _gamerStorage.GetGamerLastPendingGame(gamer.Id);
            if (game == null)
                game = _gamerStorage.CreateGame(gamer.Id);
            game.Score = 0;
            SetCurrentGame(game);
            SyncClientGames();
            _messageHandler.Publish(Topics.GameStarted, gamer);
        }

        private readonly IConfiguration _config;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IGamerContextMethods _gamerStorage;
        private readonly MqttMessageHandler _messageHandler;

        public Guid Id { get; set; }

        public Game CurrentGame { get; set; }

        public SignalRGame GetCurrentGameAsSignalR => GameToSignalR(CurrentGame);

        public SignalRGame GameToSignalR(Game currentGame)
        {
            if (currentGame == null)
                return null;
            var gamer = _gamerStorage.GetGamer(currentGame.GamerId);
            return new SignalRGame(gamer.Id, gamer.DisplayName, currentGame.Score, currentGame.Tries, currentGame.MaxTries);
        }
        public SignalRGame GameToSignalR(Game currentGame, out Gamer gamer)
        {
            gamer = null;
            if (currentGame == null)
                return null;
            gamer = _gamerStorage.GetGamer(currentGame.GamerId);
            return new SignalRGame(gamer.Id, gamer.DisplayName, currentGame.Score, currentGame.Tries, currentGame.MaxTries);
        }
        public void PostCustom(string topic, object obj)
        {
            _messageHandler.Publish(topic, obj);
        }

        public void PostString(string topic, string text)
        {
            _messageHandler.PublishPlaintext(topic, text);
        }
    }

}
