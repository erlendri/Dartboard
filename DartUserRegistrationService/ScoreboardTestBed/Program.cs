
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

            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;


            client.Subscribe("/geopackman/score");

            Gamer packmanPlayer = new Gamer()
            {
                Id = Guid.NewGuid(),
                Email ="email@gamer.com",
                FirstName="John",
                LastName="Doe"
            };

            client.Publish<Gamer>("/geopackman/score", packmanPlayer);
        }

        

        private static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.Default.GetString (e.Message);
            Console.WriteLine($"Score received: {message}");

            var wiredGamer = JsonConvert.DeserializeObject<Gamer>(message);
           
            Console.WriteLine($"Email from transferred gamer: {wiredGamer.Email}");
            Console.ReadKey();
        }
    }
}
