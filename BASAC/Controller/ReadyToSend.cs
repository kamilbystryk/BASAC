using System;
namespace BASAC.Controller
{
    public class ReadyToSend : EventArgs
    {
        private String _topic;
        private byte[] _message;

        public String Topic
        {
            get { return _topic; }
            set { _topic = value; }
        }

        public byte[] Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public ReadyToSend(String topic, byte[] message)
        {
            Topic = topic;
            Message = message;
        }
    }
}
