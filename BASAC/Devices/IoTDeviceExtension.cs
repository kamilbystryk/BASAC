using System;
using System.Collections.Generic;
using System.Linq;

namespace BASAC.Devices
{
    public static class IoTDeviceExtension
    {
        public static bool ContainsMac(this List<IoTDevice> source, string Mac)
        {
            return source.Any(x => x.MAC.Equals(Mac));
        }

        public static bool IsConnected(this List<IoTDevice> source, string Mac)
        {
            bool ret = source.Any(x => (x.Online == true && x.MAC.Equals(Mac)
            && (x.Supply == SupplyEnumeration.Main12VDC || x.Supply == SupplyEnumeration.main230VAC || x.Supply == SupplyEnumeration.main230VAC))
            || x.Supply == SupplyEnumeration.Battery);
            return ret;
        }

        public static void SetConnected(this List<IoTDevice> source, string Mac, bool ConnectionState)
        {           
            source.Where(o => o.MAC.Equals(Mac)).ToList().ForEach(f => f.Online = ConnectionState);
        }
        public static void SetConnected(this List<IoTDevice> source, string Mac)
        {
            source.Where(o => o.MAC.Equals(Mac)).ToList().ForEach(f => f.Online = true);
        }

        public static void UpdateObject(this List<IoTDevice> source, string Mac, string ObjectDesc, string Message, bool fromCloud)
        {
            //TODO
        }

        public static bool IsLocalControl(this List<IoTDevice> source, string Mac, string registerName)
        {
            return source.FirstOrDefault(o => o.MAC.Equals(Mac)).LocalControl;
        }
    }
}
