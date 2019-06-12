
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
            //MqttTests();

            //DatabaseTests();

        }

        private static void MqttTests()
        {
            var client = new MqttMessageHandler("tpg-hackathon.westeurope.cloudapp.azure.com");//new MqttMessageHandler("169.254.151.119");

            client.MqttMsgPublishReceived += PublishReceived;

            var gamerId = Guid.NewGuid();
            client.Subscribe(Topics.Gamer);

            var packmanPlayer = new Gamer()
            {
                Id = gamerId,
            };

            client.Publish<Gamer>(Topics.Gamer, packmanPlayer);

            client.Subscribe(Topics.Score);

            var score = new Score(gamerId, 1337);
            client.Publish<Score>(Topics.Score, score);
        }

        //private static void DatabaseTests()
        //{
        //    DbContextOptionsBuilder<GamerContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<GamerContext>();
        //    dbContextOptionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=NDCRegistration_db;Trusted_Connection=True;");//"Data Source = ndcregistrationdbserver.database.windows.net; Initial Catalog = NDCRegistration_db; User ID = TeleplanNDC; Password = ThreeLittlePigs3; Connect Timeout = 60; Encrypt = True; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
        //    var gamerContext = new GamerContext(dbContextOptionsBuilder.Options);
        //   // gamerContext.Database.Migrate();
        //    GamerContextMethods gamerStorage = new GamerContextMethods();

        //    var packmanPlayer2 = new Gamer()
        //    {
        //        DisplayName = "Moonwalker",
        //    };
        //    var createdGamer = gamerStorage.CreateOrUpdateGamer(packmanPlayer2);
        //    var game = new Game()
        //    {
        //        GamerId = createdGamer.Id,
        //        Score = 1337,
        //        State = GameState.Pending
        //    };

        //    var createdGame = gamerStorage.CreateGame(game);


        //    gamerStorage.CompleteGame(createdGame);
        //    //Console.Read();
        //    gamerStorage.DeleteGame(createdGame.Id);
        //}


        public static void PublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == "/geopackman/Gamer")
            {
                var message = Encoding.Default.GetString(e.Message);
                Console.WriteLine($"Gamer received: {message}");

                var wiredGamer = JsonConvert.DeserializeObject<Gamer>(message);

                Console.WriteLine($"Email from transferred gamer: {wiredGamer.DisplayName}");
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
