using Dart.Messaging;
using Dart.Messaging.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NDCRegistration.Hubs;
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
    }
    public class MqttHandler : IMqttHandler
    {
        public MqttHandler(IConfiguration config, IHubContext<MessageHub> hubContext, IGamerStorage gamerStorage)
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
            _messageHandler.MqttMsgPublishReceived += _messageHandler_MqttMsgPublishReceived;
        }

        private void _messageHandler_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var message = Encoding.Default.GetString(e.Message);
            if (e.Topic == Topics.ScoreUpdate)
            {
                var gamer = JsonConvert.DeserializeObject<GamerMinimal>(message);
                MessageHubMethods.SendGameUpdated(_hubContext, gamer.Id, gamer.Score).Wait();
            }
            else if (e.Topic == Topics.GameCompleted)
            {
                GetGamerFromMessage(message, out GamerMinimal gamer, out Gamer stored, out Game game);
                if (game == null)
                    return;
                game.Score = gamer.Score;
                _gamerStorage.CompleteGame(game);
                MessageHubMethods.SendGameCompleted(_hubContext, gamer.Id, gamer.Score).Wait();

            }
            else if (e.Topic == Topics.GameAborted)
            {
                GetGamerFromMessage(message, out GamerMinimal gamer, out Gamer stored, out Game game);
                if (game == null)
                    return;
                _gamerStorage.DeleteGame(game.Id);
                MessageHubMethods.SendGameDeleted(_hubContext, game.Id).Wait();
            }

            Console.WriteLine($"Gamer received: {message}");

        }
        private void GetGamerFromMessage(string message, out GamerMinimal gamer, out Gamer stored, out Game game)
        {
            gamer = JsonConvert.DeserializeObject<GamerMinimal>(message);
            stored = _gamerStorage.GetGamer(gamer.Id);
            game = stored.Games.OrderByDescending(f => f.Id).FirstOrDefault(f => f.State == GameState.Pending);
        }

        public void PostGameStarted(GamerMinimal gamer)
        {
            _messageHandler.Publish(Topics.GameStarted, gamer);
        }

        private readonly IConfiguration _config;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IGamerStorage _gamerStorage;
        private readonly MqttMessageHandler _messageHandler;

        public Guid Id { get; set; }
    }

}
