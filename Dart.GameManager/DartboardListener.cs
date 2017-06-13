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
    public interface IDartboardListener
    {
        event DartboardListener.ThrowReceived ThrowReceivedEvent;
        void OnThrowReceived(OnThrowReceivedArgs args);
    }

    public class DartboardListener : IDartboardListener
    {
        public MqttClient MqttClient { get; set; }
        public event ThrowReceived ThrowReceivedEvent;
        
        public delegate void ThrowReceived(object sender, OnThrowReceivedArgs args);

        private string _clientId;
        private DateTime _start = DateTime.Now;
        public virtual void OnThrowReceived(OnThrowReceivedArgs args)
        {
            ThrowReceivedEvent?.Invoke(this, args);
        }

        public DartboardListener(MqttClient mqttClient)
        {
            // create client instance
            MqttClient = mqttClient; 

           _clientId = Guid.NewGuid().ToString();
            MqttClient.Connect(_clientId);
           
           
            MqttClient.Subscribe(new[] { "dartfeed" }, new[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            MqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            MqttClient.ConnectionClosed += MqttClient_ConnectionClosed;

        }

        private void MqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            var test = DateTime.Now.Subtract(_start).TotalSeconds;
            MqttClient.Connect(_clientId);
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            byte[] messageBytes = e.Message;
            string message = Encoding.ASCII.GetString(messageBytes);

            if (string.IsNullOrEmpty(message))
                return;

            // One second bluelights blink
            MqttClient.Publish("trigger", Encoding.UTF8.GetBytes("1000"));

            int points = CalculatePoints(message);
            var throwReceivedArgs = new OnThrowReceivedArgs() {Points = points};

            OnThrowReceived(throwReceivedArgs);
        }

        public int CalculatePoints(string rawPoints)
        {
            string[] pointsArray = rawPoints.Split(';');

            int points = int.Parse(pointsArray[0]);
            points *= int.Parse(pointsArray[1]);
            
            return points;
         }
    }
}
