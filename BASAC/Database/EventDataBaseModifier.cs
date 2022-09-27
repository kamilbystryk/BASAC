using System;
namespace BASAC.Database
{
    public enum EventDataBaseModifierCode
    {
        EventDataBaseName,
        TableEvent,
        Event_Id,
        Event_Type,
        Event_Date,
        Event_Local,
        Event_Value
    }

    public class EventDataBaseModifier
    {
        public static string GetName(EventDataBaseModifierCode code)
        {
            switch (code)
            {
                default:
                    return null;
                case EventDataBaseModifierCode.EventDataBaseName:
                    return "Events.db";
                case EventDataBaseModifierCode.TableEvent:
                    return "Events";
                case EventDataBaseModifierCode.Event_Id:
                    return "Id";
                case EventDataBaseModifierCode.Event_Type:
                    return "Type";
                case EventDataBaseModifierCode.Event_Date:
                    return "Date";
                case EventDataBaseModifierCode.Event_Local:
                    return "Local";
                case EventDataBaseModifierCode.Event_Value:
                    return "Value";
            }
        }
    }
}

