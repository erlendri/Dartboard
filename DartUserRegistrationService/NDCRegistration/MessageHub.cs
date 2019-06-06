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
            _gamerStorage.DeleteGame(id);
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
    }
}