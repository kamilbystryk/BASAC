using System;
namespace BASAC.Devices
{
    public class DeviceTimer
    {
        int _time;
        String _MAC;
        public DeviceTimer(String MAC, int time)
        {
            this._MAC = MAC;
            this._time = time;
        }
        public int time
        {
            get { return _time; }
            set { _time = value; }
        }
        public String MAC
        {
            get { return _MAC; }
            set { _MAC = value; }
        }
    }
}