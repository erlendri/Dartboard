using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Dart.GameManager
{
    class Program
    {
        public static BeerPublisher MyBeerPublisher { get; set; } 

        static void Main(string[] args)
        {
            var mqttClient = new MqttClient("192.168.2.66");
            var myDartListener = new DartboardListener(mqttClient);
            MyBeerPublisher = new BeerPublisher(mqttClient);

            IGameManager myGameManager = new GameManager(myDartListener, MyBeerPublisher);

            myGameManager.GameScoreUpdated += MyGameManager_GameScoreUpdated;
            
            Console.ReadKey();
        }

        private static void MyGameManager_GameScoreUpdated(object sender, OnScoreUpdatedArgs args)
        {
            Console.WriteLine($"Current game score:");
            Console.WriteLine($"First Throw: {args.CurrentGame.FirstThrow}. Second throw: {args.CurrentGame.SecondThrow}. Third throw: {args.CurrentGame.ThirdThrow}");
            Console.WriteLine($"Total score: {args.CurrentGame.TotalScore}\n\n");

        }
    }
}
