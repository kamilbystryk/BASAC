
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BASAC.Database;
using BASAC.Devices;
using BASAC.MQTT;

namespace BASAC
{
    public class Controller
    {


        private static Controller _instance;
        private static readonly object Lock = new object();

        System.Timers.Timer secondTimer;
        static List<DeviceTimer> _DeviceTimer;

         List<IoTDevice> IotDevicesList;
        List<IoTDevice> IotNewDevicesList;

        public event EventHandler<ReadyToSend> mReadyToSend;

        public event EventHandler<IotDeviceNotify> mIotDeviceNotify;

        public static Controller Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                        _instance = new Controller();
                }
                return _instance;
            }
        }

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

        public class ReadyToReceive : EventArgs
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

            public ReadyToReceive(String topic, byte[] message)
            {
                Topic = topic;
                Message = message;
            }
        }

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


        public Controller()
        {
            IotDevicesList = new List<IoTDevice>();
            IotNewDevicesList = new List<IoTDevice>();
            _DeviceTimer = new List<DeviceTimer>();
            InitIotDevicesList();
        }

        private void InitIotDevicesList()
        {
            var x0 = IotDevicesDataBase.Instance.DBGetAllIoTDevices();
            foreach (var item in x0)
            {
                IotDevicesList.Add(item);
            }
            List<String> temp = new List<String>();
            foreach (var item in IotDevicesList)
            {
                temp.Add(item.MAC);
            }
            SetDeviceTimer(temp);
        }

        private void SetDeviceTimer(List<string> MACs)
        {
            _DeviceTimer.Clear();
            foreach (var item in MACs)
            {
                _DeviceTimer.Add(new DeviceTimer(item, 0));
            }
        }

        public void MqttIotDeviceNotify(String MAC, String obj, byte[] message, bool FromCloud)
        {
            //TODO obsługa zgloszeń z urządzeń

            if (IotDevicesList.ContainsMac(MAC))
            {
                ResetDeviceTimeout(MAC);
                if (!IotDevicesList.IsConnected(MAC))
                {

                }
                else
                {

                }
            }
            else if (!IotNewDevicesList.ContainsMac(MAC))
            {
                if (mReadyToSend != null)
                {
                    mReadyToSend.Invoke(this, new ReadyToSend(MQTTenum.IoTserviceTopic + "/" + MAC + "/down", Encoding.ASCII.GetBytes("GiveMeAllRegisters")));
                }
            }
        }

        private void ResetDeviceTimeout(string mAC)
        {
            foreach(var item in _DeviceTimer)
            {
                if (item.MAC.Equals(mAC))
                {
                    item.time = 0;
                }
            }
        }








        public void IotDeviceServiceEvent(String MAC, byte[] message, bool FromCloud)
        {
            
        }

        internal bool IotDeviceMACexists(string mac)
        {
            bool ret = false;
            var listCopy = new List<IoTDevice>(IotDevicesList);
            foreach (var item in listCopy)
            {
                if (item.MAC.Equals(mac))
                {
                    ret = true;
                }
            }
            return ret;
        }

        internal bool NewIotDeviceMACexists(string mac)
        {
            bool ret = false;
            foreach (var item in IotNewDevicesList)
            {
                if (item.MAC.Equals(mac))
                {
                    ret = true;
                }
            }
            return ret;
        }

        internal bool AddDeviceToNewDeviceList(IoTDevice node)
        {
            if (!NewIotDeviceMACexists(node.MAC))
            {
                IotNewDevicesList.Add(node);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
