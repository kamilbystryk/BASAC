﻿using System;
using System.IO;

namespace BASAC.Database
{
    public enum IotDevicesDataBaseModifierCode
    {
        IotDevdatabasename,
        TableDev,
        Dev_MAC,
        Dev_ID,
        Dev_RegisterId,
        Dev_Name,
        Dev_Desc,
        Dev_CoilType,
        Dev_deleted,
        Dev_value,
        Dev_acq,
        Dev_local,
        Dev_devicetype,
        Dev_supply,
        Dev_CommCh,

        TableLoc2Nam,
        TableLoc2NamId,
        TableLoc2NamName,
        TableLoc2NamDesc
    }



    public class IotDevicesDataBaseModifier
    {
        public static string GetName(IotDevicesDataBaseModifierCode code)
        {
            switch (code)
            {
                default:
                    return null;
                case IotDevicesDataBaseModifierCode.IotDevdatabasename:
                    return "IotDevDatabase.db";
                case IotDevicesDataBaseModifierCode.TableDev:
                    return "IoTdevices";
                case IotDevicesDataBaseModifierCode.Dev_MAC:
                    return "MAC";
                case IotDevicesDataBaseModifierCode.Dev_Name:
                    return "Name";
                case IotDevicesDataBaseModifierCode.Dev_ID:
                    return "ID";
                case IotDevicesDataBaseModifierCode.Dev_RegisterId:
                    return "Reg_ID";
                case IotDevicesDataBaseModifierCode.Dev_Desc:
                    return "Desc";
                case IotDevicesDataBaseModifierCode.Dev_CoilType:
                    return "CoilType";
                case IotDevicesDataBaseModifierCode.Dev_deleted:
                    return "Deleted";
                case IotDevicesDataBaseModifierCode.Dev_acq:
                    return "Acq";
                case IotDevicesDataBaseModifierCode.Dev_local:
                    return "Dev_local";
                case IotDevicesDataBaseModifierCode.Dev_supply:
                    return "Dev_supply";
                case IotDevicesDataBaseModifierCode.Dev_value:
                    return "Value";
                case IotDevicesDataBaseModifierCode.Dev_devicetype:
                    return "Dev_Type";
                case IotDevicesDataBaseModifierCode.Dev_CommCh:
                    return "CommCh";
                case IotDevicesDataBaseModifierCode.TableLoc2Nam:
                    return "TableLoc2Nam";
                case IotDevicesDataBaseModifierCode.TableLoc2NamId:
                    return "ID";
                case IotDevicesDataBaseModifierCode.TableLoc2NamName:
                    return "Name";
                case IotDevicesDataBaseModifierCode.TableLoc2NamDesc:
                    return "Desc"; 


            }

        }



    }
}
