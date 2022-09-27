using System;
namespace BASAC.Devices
{
    public class DeviceLocal
    {
        int _id;
        String _name;
        String _desc;
        public DeviceLocal()
        {
            this._id = 0;
            this._name = "";
            this._desc = "";
        }

        public DeviceLocal(int ID, String Name, String Description)
        {
            this._id = ID;
            this._name = Name;
            this._desc = Description;
        }
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public String Description
        {
            get { return _desc; }
            set { _desc = value; }
        }
    }

}
