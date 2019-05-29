using Dart.Messaging;
using Dart.Messaging.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NDCRegistration.Hubs;
using Newtonsoft.Json;
using System;
using System.Text;

namespace NDCRegistration
{
    public interface IMqttHandler
    {
        Guid Id { get; set; }
        void PostGameStart(GamerMinimal gamer);
    }
    public class MqttHandler : IMqttHandler
    {
        public MqttHandler(IConfiguration config, IHubContext<MessageHub> hubContext)
        {
            Id = Guid.NewGuid();
            _config = config;
            _hubContext = hubContext;
            var key = _config.GetValue<string>("MqttSettings:BrokerUri");
            _messageHandler = new MqttMessageHandler(key);
            _messageHandler.Subscribe(Topics.GameStart);
            _messageHandler.MqttMsgPublishReceived += _messageHandler_MqttMsgPublishReceived;
        }

        private void _messageHandler_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var message = Encoding.Default.GetString(e.Message);
            Console.WriteLine($"Gamer received: {message}");

            var wiredGamer = JsonConvert.DeserializeObject<GamerMinimal>(message);

            _hubContext.Clients.All.SendAsync("GameStarted", wiredGamer);
        }

        public void PostGameStart(GamerMinimal gamer)
        {
            _messageHandler.Publish(Topics.GameStart, gamer);
        }

        private readonly IConfiguration _config;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly MqttMessageHandler _messageHandler;

        public Guid Id { get; set; }
    }

}
