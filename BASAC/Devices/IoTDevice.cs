using System;
using System.Collections.Generic;

namespace BASAC.Devices
{
    public class IoTDevice
    {
        private int _id;
        private String _MAC;
        private int _state;
        private bool _online = false;
        private List<IoTregister> _registers;
        private int _local;
        private int _room;
        private String _deviceModel;
        private SupplyEnumeration _supply;
        private bool _localControl;

        public IoTDevice()
        {
            this._id = -1;
            this._MAC = "";
            this._state = -1;
            this._online = false;
            this._registers = new List<IoTregister>();
            this._local = 0;
            this._room = 0;
            this._deviceModel = "UNK";
            this._supply = 0;
            this._localControl = false;
        }

        public IoTDevice(int Id, String MAC, int State, bool Online, List<IoTregister> Registers, int Localisation, int Room, DeviceType DeviceModel, 
                SupplyEnumeration Supply, bool LocalControl)
        {
            this._id = Id;
            this._MAC = MAC;
            this._state = State;
            this._online = Online;
            this._registers = Registers;
            this._local = Localisation;
            this._room = Room;
            this._deviceModel = "DeviceModel";
            this._supply = Supply;
            this._localControl = LocalControl;
        }

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string MAC
        {
            get { return _MAC; }
            set { _MAC = value; }
        }

        public int State
        {
            get { return _state; }
            set { _state = value; }
        }

        public bool Online
        {
            get { return _online; }
            set { _online = value; }
        }

        public List<IoTregister> Registers
        {
            get { return _registers; }
            set { _registers = value; }
        }

        public int Localisation
        {
            get { return _local; }
            set { _local = value; }
        }

        public int Room
        {
            get { return _room; }
            set { _room = value; }
        }

        public string DeviceModel
        {
            get { return _deviceModel; }
            set { _deviceModel = value; }
        }

        public SupplyEnumeration Supply
        {
            get { return _supply; }
            set { _supply = value; }
        }

        public bool LocalControl
        {
            get { return _localControl; }
            set { _localControl = value; }
        }
    }
}
