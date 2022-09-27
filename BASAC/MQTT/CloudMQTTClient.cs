using System;
using uPLibrary.Networking.M2Mqtt;

namespace BASAC.MQTT
{
    public class CloudMQTTClient
    {
        private static CloudMQTTClient _instance;
        private static readonly object Lock = new object();
        static MqttClient client;

        public static CloudMQTTClient Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                        _instance = new CloudMQTTClient();
                }
                return _instance;
            }
        }

        public CloudMQTTClient()
        {
        }

        public void Connect()
        {
        }
    }
}
