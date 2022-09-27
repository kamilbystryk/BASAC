using System;
namespace BASAC.Devices
{
    public class DeviceHelper
    {
        public DeviceHelper()
        {
        }

        internal static int GetModelFromName(string code)
        {
            switch (code)
            {
                default:
                    return -1;
                case "BWX1":
                    return 1;
                case "BWX2":
                    return 2;
                case "BWX3":
                    return 3;
                case "BWX4":
                    return 4;
            }

        }

    }
}
