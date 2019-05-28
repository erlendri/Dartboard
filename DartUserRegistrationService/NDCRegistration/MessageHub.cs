using Dart.Messaging.Models;
using Microsoft.AspNetCore.SignalR;
using NDCRegistration.MessageHubModels;
using System;
using System.Threading.Tasks;

namespace NDCRegistration.Hubs
{
    public class MessageHub : Hub
    {
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
            await GameStarted(new Gamer
            {
                Id = gamerId,
                DisplayName = gamerId.ToString(),
                FirstName = "Hello",
                LastName = "World",
                Email = "test@test.com"
            });
        }
        public async Task GameStarted(Gamer gamer)
        {
            await Clients.All.SendAsync("GameStarted", gamer);
        }
    }
}