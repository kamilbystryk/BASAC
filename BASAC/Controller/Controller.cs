
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BASAC.Database;
using BASAC.Devices;
using BASAC.MQTT;
using BASAC.Util;


namespace BASAC.Controller
{
    public class Controller
    {


        private static Controller _instance;
        private static readonly object Lock = new object();

        System.Timers.Timer secondTimer;
        static List<DeviceTimer> _DeviceTimer;

        public List<IoTDevice> IotDevicesList;
        public List<IoTDevice> IotNewDevicesList;

        public event EventHandler<ReadyToSend> mReadyToSend;
        public event EventHandler<ReadyTosendCloud> mReadyToSendCloud;
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

        public void MqttIotDeviceNotify(String MAC, String Register, byte[] message, bool FromCloud)
        {
            //TODO config and firmware messages

            if (IotDevicesList.ContainsMac(MAC))
            {

                _DeviceTimer.ResetDeviceTimeout(MAC);
                String _mess = Encoding.UTF8.GetString(message).RemoveCharFromString('\0');
                if (Encoding.UTF8.GetString(message).StartsWith("@") && Register.StartsWith("OUT"))
                {
                    if (IotDevicesList.IsLocalControl(MAC, Register))
                        _mess = _mess.Substring(1);
                }

                if (!IotDevicesList.IsConnected(MAC))
                {
                    IotDevicesList.SetConnected(MAC, true);
                }
                else
                {
                    IoTDeviceExtension.UpdateObject(IotDevicesList, MAC, Register, _mess, FromCloud);
                    if (!FromCloud)
                    {
                        if (mReadyToSendCloud != null)
                            mReadyToSendCloud(this, new ReadyTosendCloud("nodeswifi/" + MAC + "/" + Register, message));
                    }
                    else
                    {
                        if (mReadyToSendCloud != null)
                            mReadyToSendCloud(this, new ReadyTosendCloud("nodeswifi/" + MAC + "/" + Register, message));
                    }
                }
            }
            else if (!IotNewDevicesList.ContainsMac(MAC))
            {
                if (mReadyToSend != null)
                { 
                    mReadyToSend.Invoke(this, 
                        new ReadyToSend(MQTTenum.IoTserviceTopic + "/" + MAC + "/down", Encoding.ASCII.GetBytes(MQTTenum.MQTTCommanddGiveMeAllRegisters)));
                }
            }
        }

        public void IotDeviceServiceEvent(String MAC, byte[] message, bool FromCloud)
        {
            
        }

        internal bool AddDeviceToNewDeviceList(IoTDevice node)
        {
            if (!IotNewDevicesList.ContainsMac(node.MAC))
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
