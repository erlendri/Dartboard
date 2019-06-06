
using System;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using Dart.Messaging;
using Dart.Messaging.Models;

namespace ScoreboardTestBed
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MqttMessageHandler("tpg-hackathon.westeurope.cloudapp.azure.com");

            client.MqttMsgPublishReceived += PublishReceived;            
            client.Subscribe(Topics.ScoreUpdate);

            var gamerMini = new GamerMinimal
            {
                Id = Topics.TestId,
                Name = "Hello world",
                MaxTries = 3,
            };
            var score = 123;
            var random = new Random();
            while (score != 0)
            {
                var key = Console.ReadLine();
                gamerMini.Score = random.Next(200, 50000);

                client.Publish<GamerMinimal>(Topics.ScoreUpdate, gamerMini);

            }

        }

        

        public static void PublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            //if (e.Topic == "/geopackman/Gamer")
            //{
            //    var message = Encoding.Default.GetString(e.Message);
            //    Console.WriteLine($"Gamer received: {message}");

            //    var wiredGamer = JsonConvert.DeserializeObject<Gamer>(message);

            //    Console.WriteLine($"Email from transferred gamer: {wiredGamer.Email}");
            //}
            //else
            //{
            //    var message = Encoding.Default.GetString(e.Message);
            //    Console.WriteLine($"Score received: {message}");

            //    var wiredScore = JsonConvert.DeserializeObject<Score>(message);

            //    Console.WriteLine($"Score for transferred gamer: {wiredScore.GameScore}");
            //}
            //Console.WriteLine("\n");
            var message = Encoding.Default.GetString(e.Message);
            Console.WriteLine($"Gamer received: {message}");

        }
    }
}
