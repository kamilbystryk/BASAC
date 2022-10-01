using System;
namespace BASAC.Controller
{
    public class IotDeviceNotify : EventArgs
    {
        private String _mac;
        private String _Registername;
        private byte[] _message;
        private bool _fromcloud;

        public String Mac
        {
            get { return _mac; }
            set { _mac = value; }
        }

        public String Registername
        {
            get { return Registername; }
            set { _Registername = value; }
        }

        public byte[] message
        {
            get { return _message; }
            set { _message = value; }
        }

        public bool fromcloud
        {
            get { return _fromcloud; }
            set { _fromcloud = value; }
        }

        public IotDeviceNotify(String MAC, String RegisterName, byte[] Message, bool FromCloud)
        {
            Mac = MAC;
            Registername = RegisterName;
            message = Message;
            fromcloud = FromCloud;
        }
    }

}
