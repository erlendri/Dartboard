using System;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Dart.GameManager
{
    public class BeerPublisher : IBeerPublisher
    {
        public MqttClient MqttClient { get; set; }
        
        public BeerPublisher(MqttClient mqttClient)
        {
            // create client instance
            MqttClient = mqttClient;

            string clientId = Guid.NewGuid().ToString();
            MqttClient.Connect(clientId);    
        }


        public void PourBeer() 
        {
            MqttClient.Publish("servo", Encoding.UTF8.GetBytes("1000"));
        }
        
    }
}
