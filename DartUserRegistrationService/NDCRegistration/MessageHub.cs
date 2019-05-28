using Dart.Messaging.Models;
using Microsoft.AspNetCore.SignalR;
using NDCRegistration.MessageHubModels;
using System;
using System.Threading.Tasks;

namespace NDCRegistration.Hubs
{
    public class MessageHub : Hub
    {
        public Guid Id { get; }
        private IMqttHandler MqttHandler { get; }

        public MessageHub(IMqttHandler mqttHandler)
        {
            Id = Guid.NewGuid();
            MqttHandler = mqttHandler;
        }
        public async Task SendMessage(ScoreMessage message)
        {
            //newtonsoft JsonConvert
            await Clients.All.SendAsync("ScoreMessage", "hello world");
        }
        public async Task HelloServer()
        {
            await Task.CompletedTask;
        }
        public async Task StartGame(Guid gamerId)
        {
            var gamer = new Gamer
            {
                Id = gamerId,
                DisplayName = gamerId.ToString(),
                FirstName = "Hello",
                LastName = "World",
                Email = "test@test.com"
            };
            await Task.Run(() =>
            {
                MqttHandler.PostGameStart(gamer);
            });
            //await GameStarted(gamer);
        }
        public async Task GameStarted(Gamer gamer)
        {
            await Clients.All.SendAsync("GameStarted", gamer);
        }
    }
}