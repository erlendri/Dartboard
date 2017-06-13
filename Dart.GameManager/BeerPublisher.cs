using System;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Dart.GameManager
{
    public class BeerPublisher : IBeerPublisher
    {
        public bool IsDisconecting { get; set; } = false;
        public string ClientId { get; set; }

        public MqttClient MqttClient { get; set; }
        
        public BeerPublisher(MqttClient mqttClient)
        {
            MqttClient = mqttClient;

            ClientId = Guid.NewGuid().ToString();
            MqttClient.ConnectionClosed += MqttClient_ConnectionClosed;
            ConnectClient();    
        }
        

        public void Disconnect()
        {
            if (MqttClient == null)
                return;

            IsDisconecting = true;
            
            if (MqttClient.IsConnected)
            MqttClient.Disconnect();
            IsDisconecting = false;
        }

        public void ConnectClient()
        {
            MqttClient.Connect(ClientId, "", "", false, ushort.MaxValue);
        }

        public void PourBeer() 
        {
            // One second bluelights blink
            MqttClient.Publish("trigger", Encoding.UTF8.GetBytes("3000"));
            MqttClient.Publish("servo", Encoding.UTF8.GetBytes("1000"));
        }

        private void MqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection closed...");
        }
    }
}
