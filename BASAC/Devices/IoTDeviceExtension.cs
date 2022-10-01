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

        public static bool MacExists(this List<IoTDevice> source, string Mac)
        {
            bool ret = false;
            var listCopy = new List<IoTDevice>(source);
            foreach (var item in listCopy)
            {
                if (item.MAC.Equals(Mac))
                {
                    ret = true;
                }
            }
            return ret;
        }
    }
}
