﻿using Dart.Messaging;
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
                _testTriesCounter = 0;
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
            //else
            //{
            //    if (float.TryParse(qr, out float scannedQr))
            //    {
            //        //get the qr from the api
            //        var uri = ApiUri + qr;
            //        var client = new HttpClient();
            //        await Task.Run(() =>
            //        {
            //            var res = client.GetAsync(uri).Result;
                        
            //            if (res.IsSuccessStatusCode)
            //            {

            //                var strResult = res.Content.ReadAsStringAsync().Result;
            //                var parsed = ApiResponse.FromJson(strResult);
            //                gamer = new Gamer
            //                {
            //                    QrCode = qr,
            //                    DisplayName = parsed.FirstName != null && parsed.FirstName.Length > 0 ?
            //                    $"{parsed.FirstName.Substring(0, 1)}. {parsed.Surname}" : "Anonymous"
            //                };
            //                Clients.Caller.SendAsync(SignalRTopics.UserLookup, gamer);
            //            }

            //        });
            //    }
            //}
        }
        private static int _testTriesCounter = 0;
        public async Task TestUpdateCurrent()
        {
            await Task.Run(() =>
            {
                var currentGame = _mqttHandler.GameToSignalR(_mqttHandler.CurrentGame, out Gamer gamer);
                if (currentGame == null)
                    return;
                var gamerMini = gamer.ToMinimal();
                _testTriesCounter++;
                gamerMini.Tries = _testTriesCounter;
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
                gamerMini.MaxTries = 3;
                gamerMini.Tries = 3;
                _mqttHandler.PostCustom(Topics.ScoreUpdate, gamerMini);
            });

        }
    }
}