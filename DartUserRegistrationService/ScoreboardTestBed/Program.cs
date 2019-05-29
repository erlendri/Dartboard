
using System;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using Dart.Messaging;
using Dart.Messaging.Models;
using NDCRegistration;
using Microsoft.EntityFrameworkCore;

namespace ScoreboardTestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            MqttTests();

            DatabaseTests();

        }

        private static void MqttTests()
        {
            var client = new MqttMessageHandler("tpg-hackathon.westeurope.cloudapp.azure.com");

            client.MqttMsgPublishReceived += PublishReceived;

            var gamerId = Guid.NewGuid();
            client.Subscribe(Topics.Gamer);

            var packmanPlayer = new Gamer()
            {
                Id = gamerId,
                Email = "email@gamer.com",
                FirstName = "John",
                LastName = "Doe"
            };

            client.Publish<Gamer>(Topics.Gamer, packmanPlayer);

            client.Subscribe(Topics.Score);

            var score = new Score(gamerId, 1337);
            client.Publish<Score>(Topics.Score, score);
        }

        private static void DatabaseTests()
        {
            DbContextOptionsBuilder<GamerContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<GamerContext>();
            dbContextOptionsBuilder.UseSqlServer("Data Source = ndcregistrationdbserver.database.windows.net; Initial Catalog = NDCRegistration_db; User ID = TeleplanNDC; Password = ThreeLittlePigs3; Connect Timeout = 60; Encrypt = True; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
            var gamerContext = new GamerContext(dbContextOptionsBuilder.Options);

            LiveGamerStorage gamerStorage = new LiveGamerStorage(gamerContext);

            var packmanPlayer2 = new Gamer()
            {
                Email = "email@gamer.com",
                DisplayName = "Tetris",
                FirstName = "John",
                LastName = "Doe"
            };
            var createdGamer = gamerStorage.CreateOrUpdateGamer(packmanPlayer2);
            var game = new Game()
            {
                GamerId = createdGamer.Id,
                Score = 34,
                State = GameState.Pending
            };

            var createdGame = gamerStorage.CreateGame(game);


            gamerStorage.CompleteGame(createdGame);
            Console.Read();
            gamerStorage.DeleteGame(createdGame.Id);
        }


        public static void PublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == "/geopackman/Gamer")
            {
                var message = Encoding.Default.GetString(e.Message);
                Console.WriteLine($"Gamer received: {message}");

                var wiredGamer = JsonConvert.DeserializeObject<Gamer>(message);

                Console.WriteLine($"Email from transferred gamer: {wiredGamer.Email}");
            }
            else
            {
                var message = Encoding.Default.GetString(e.Message);
                Console.WriteLine($"Score received: {message}");

                var wiredScore = JsonConvert.DeserializeObject<Score>(message);

                Console.WriteLine($"Score for transferred gamer: {wiredScore.GameScore}");
            }
            Console.WriteLine("\n");
           
        }
    }
}
