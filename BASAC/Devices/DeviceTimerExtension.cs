using System;
using System.Collections.Generic;

namespace BASAC.Devices
{
    public static class DeviceTimerExtension
    {
        public static void ResetDeviceTimeout(this List<DeviceTimer> source, string mAC)
        {
            foreach (var item in source)
            {
                if (item.MAC.Equals(mAC))
                {
                    item.time = 0;
                }
            }
        }
    }
}
