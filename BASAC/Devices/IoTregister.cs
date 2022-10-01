using System;
namespace BASAC.Devices
{
    public class IoTregister
    {
        private string _Dev_MAC;
        private int _Dev_ID;
        private int _Dev_RegisterId;
        private string _Dev_Name;
        private string _Dev_Desc;
        private string _Dev_CoilType;
        private bool _Dev_deleted;
        private string _Dev_value;
        private bool _Dev_acq;
        private int _Dev_local;
        private int _Dev_room;
        private DeviceType _Dev_devicetype;
        private SupplyEnumeration _Dev_supply;
        private ComChEnumeration _Dev_CommCh;

        public IoTregister()
        {
            this._Dev_MAC = "";
            this._Dev_ID = -1;
            this._Dev_RegisterId = -1;
            this._Dev_Name = "";
            this._Dev_Desc = "";
            this._Dev_CoilType = "";
            this._Dev_deleted = false;
            this._Dev_value = "";
            this._Dev_acq = false;
            this._Dev_local = 0;
            this._Dev_room = 0;
            this._Dev_devicetype = DeviceType.Unknown;
            this._Dev_supply = SupplyEnumeration.Unknown;
            this._Dev_CommCh = ComChEnumeration.Unknown;
        }

        public IoTregister(string MAC, int Id, int RegisterId, string Name, string Description, string CoilType, bool Deleted, 
                string Value, bool Acquisition, int Localisation, int Room, DeviceType DeviceType, SupplyEnumeration Supply, ComChEnumeration CommunicationChannel)
        {
            this._Dev_MAC = MAC;
            this._Dev_ID = Id;
            this._Dev_RegisterId = RegisterId;
            this._Dev_Name =Name;
            this._Dev_Desc = Description;
            this._Dev_CoilType = CoilType;
            this._Dev_deleted = Deleted;
            this._Dev_value = Value;
            this._Dev_acq = Acquisition;
            this._Dev_local = Localisation;
            this._Dev_room = Room;
            this._Dev_devicetype = DeviceType;
            this._Dev_supply = Supply;
            this._Dev_CommCh = CommunicationChannel;
        }

        public int Id
        {
            get { return _Dev_ID; }
            set { _Dev_ID = value; }
        }

        public string MAC
        {
            get { return _Dev_MAC; }
            set { _Dev_MAC = value; }
        }

        public int RegisterId
        {
            get { return _Dev_RegisterId; }
            set { _Dev_RegisterId = value; }
        }

        public string Name
        {
            get { return _Dev_Name; }
            set { _Dev_Name = value; }
        }

        public string Description
        {
            get { return _Dev_Desc; }
            set { _Dev_Desc = value; }
        }

        public string CoilType
        {
            get { return _Dev_CoilType; }
            set { _Dev_CoilType = value; }
        }

        public bool Deleted
        {
            get { return _Dev_deleted; }
            set { _Dev_deleted = value; }
        }

        public string Value
        {
            get { return _Dev_value; }
            set { _Dev_value = value; }
        }

        public bool Acquisition
        {
            get { return _Dev_acq; }
            set { _Dev_acq = value; }
        }

        public int Localisation
        {
            get { return _Dev_local; }
            set { _Dev_local = value; }
        }

        public int Room
        {
            get { return _Dev_room; }
            set { _Dev_room = value; }
        }

        public DeviceType DeviceType
        {
            get { return _Dev_devicetype; }
            set { _Dev_devicetype = value; }
        }

        public SupplyEnumeration Supply
        {
            get { return _Dev_supply; }
            set { _Dev_supply = value; }
        }
        public ComChEnumeration CommunicationChannel
        {
            get { return _Dev_CommCh; }
            set { _Dev_CommCh = value; }
        }

    }
}
