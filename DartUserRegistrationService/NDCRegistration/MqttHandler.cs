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
            _messageHandler.Subscribe(Topics.GameCompleted);
            _messageHandler.Subscribe(Topics.GameAborted);
            _messageHandler.Subscribe(Topics.ScoreUpdate);
            //_messageHandler.Subscribe(Topics.GameStarted);
            _messageHandler.MqttMsgPublishReceived += _messageHandler_MqttMsgPublishReceived;
        }
        public void SyncClientGames()
        {
            var gamers = _gamerStorage.GetGamers();
            if (CurrentGame != null)
            {
                var currentGame = gamers.SelectMany(f => f.Games).FirstOrDefault(g => g.Id == CurrentGame.Id);
                if (currentGame != null && currentGame.State != GameState.Pending)
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
                GetGamerFromMessage(message, out GamerMinimal gamer, out Gamer stored, out Game game);
                if (game == null)
                    return;
                _gamerStorage.UpdateGameScore(game.Id, gamer.Score);
                game.Score = gamer.Score;
                if (gamer.MaxTries <= 0)
                    CompleteGame(gamer, game);
                else
                {

                    SetCurrentGame(game);
                    MessageHubMethods.SendGameUpdated(_hubContext, stored, gamer.Score).Wait();
                }
            }
            else if (e.Topic == Topics.GameCompleted)
            {
                GetGamerFromMessage(message, out GamerMinimal gamer, out Gamer stored, out Game game);
                CompleteGame(gamer, game);

            }
            else if (e.Topic == Topics.GameAborted)
            {
                GetGamerFromMessage(message, out GamerMinimal gamer, out Gamer stored, out Game game);
                if (game != null)
                {
                    _gamerStorage.DeleteGame(game.Id);
                }
                CurrentGame = null;
                SyncClientGames();
            }

        }

        private void CompleteGame(GamerMinimal gamer, Game game)
        {
            if (game != null)
            {
                game.Score = gamer.Score;
                _gamerStorage.CompleteGame(game);
            }
            CurrentGame = null;
            SyncClientGames();
        }

        private void SetCurrentGame(Game game)
        {
            if (CurrentGame != null && CurrentGame.Id != game.Id)
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
            game = stored.Games.OrderByDescending(g=>g.DateCreated).First(g=>g.State == GameState.Pending);
        }

        public void PostGameStarted(GamerMinimal gamer)
        {
            var game = _gamerStorage.GetGamerLastPendingGame(gamer.Id);
            if (game.Score > 0)
            {
                _gamerStorage.CompleteGame(game);
                game = _gamerStorage.CreateGame(game.GamerId);
            }
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
            return new SignalRGame(gamer.Id, gamer.DisplayName, currentGame.Score);
        }
        public SignalRGame GameToSignalR(Game currentGame, out Gamer gamer)
        {
            gamer = null;
            if (currentGame == null)
                return null;
            gamer = _gamerStorage.GetGamer(currentGame.GamerId);
            return new SignalRGame(gamer.Id, gamer.DisplayName, currentGame.Score);
        }
        public void PostCustom(string topic, object obj)
        {
            _messageHandler.Publish(topic, obj);
        }
    }

}
