using System;
namespace BASAC.Devices
{
    public enum SupplyEnumeration
    {
        Unknown = -1,
        MainGeneric = 0,
        main230VAC = 1,
        Main12VDC = 2,
        Battery = 3
    }

    public enum ComChEnumeration
    {
        Unknown = -1,
        Wifi = 0,
        Eth = 1,
        BLE = 2
    }

    public enum DeviceType
    {
        Unknown = -1,
        BWX1 = 1,
        BWX2 = 2,
        BWX3 = 3
    }
}