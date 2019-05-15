
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
            var client = new MqttMessageHandler("127.0.0.1");

            client.MqttMsgPublishReceived += PublishReceived;

            var gamerId = Guid.NewGuid();
            client.Subscribe(Topics.Gamer);

            var packmanPlayer = new Gamer()
            {
                Id = gamerId,
                Email ="email@gamer.com",
                FirstName="John",
                LastName="Doe"
            };

            client.Publish<Gamer>(Topics.Gamer, packmanPlayer);
            
            client.Subscribe(Topics.Score);

            var score = new Score(gamerId, 1337);
            client.Publish<Score>(Topics.Score, score);
           
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
            Console.ReadKey();
        }
    }
}
