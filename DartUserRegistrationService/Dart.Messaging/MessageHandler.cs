using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static uPLibrary.Networking.M2Mqtt.MqttClient;

namespace Dart.Messaging
{
    public interface IMessageHandler
    {
        void Subscribe(string topic);
        void Publish<T>(string topic, T payload);
    }

    public class MqttMessageHandler : IMessageHandler
    {
        private MqttClient myClient;
        private readonly string _clientId;
        public MqttMessageHandler(string uri)
        {
            myClient = new MqttClient(uri);

            _clientId = Guid.NewGuid().ToString();
            myClient.Connect(_clientId);
        }


        public event MqttMsgPublishEventHandler MqttMsgPublishReceived
        {
            add
            {   
                    myClient.MqttMsgPublishReceived += value;   
            }
            remove
            {
                myClient.MqttMsgPublishReceived -= value;
            }
        }


        public void Publish<T>(string topic, T payload)
        {
            if (!myClient.IsConnected)
                myClient.Connect(_clientId);
            var serializedPayload = JsonConvert.SerializeObject(payload);
            myClient.Publish(topic, Encoding.UTF8.GetBytes(serializedPayload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        public void Subscribe(string topic)
        {
            myClient.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        public void Disconnect()
        {
            myClient.Disconnect();
        }

        public void PublishPlaintext(string topic, string text)
        {
            myClient.Publish(topic, Encoding.UTF8.GetBytes(text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }
    }
}
