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
        private readonly IMqttHandler _mqttHandler;
        private readonly IGamerStorage _gamerStorage;

        public MessageHub(IMqttHandler mqttHandler, IGamerStorage gamerStorage)
        {
            Id = Guid.NewGuid();
            _mqttHandler = mqttHandler;
            _gamerStorage = gamerStorage;
            
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
            _gamerStorage.DeleteGame(id);
            await MessageHubMethods.SendGameDeleted((IHubContext<MessageHub>)Context, id);

        }
    }
}