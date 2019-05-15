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

        public MqttMessageHandler(string uri)
        {
            myClient = new MqttClient(uri);

            string clientId = Guid.NewGuid().ToString();
            myClient.Connect(clientId);
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

        public int MyProperty { get; set; }

        public void Publish<T>(string topic, T payload)
        {
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
    }
}
