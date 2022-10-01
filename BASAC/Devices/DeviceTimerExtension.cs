using System;
using System.Collections.Generic;
using System.Linq;

namespace BASAC.Devices
{
    public static class DeviceTimerExtension
    {
        public static void ResetDeviceTimeout(this List<DeviceTimer> source, string Mac)
        {
            source.Where(o => o.MAC.Equals(Mac)).ToList().ForEach(f => f.time = 0);

            /*foreach (var item in source)
            {
                if (item.MAC.Equals(mAC))
                {
                    item.time = 0;
                }
            }*/
        }
    }
}
