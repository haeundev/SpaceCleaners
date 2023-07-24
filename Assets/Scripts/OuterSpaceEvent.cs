using System;
using DataTables;
using UnityEngine;

public static class OuterSpaceEvent
{
    public static event Action PlayerPositionWrapped = delegate { };
    public static event Action PlayerEnterPlanet = delegate { };
    public static event Action PlayerRefusePlanet = delegate { };
    public static event Action<bool> OnBoost = delegate { };
    public static event Action<GadgetInfo> OnGadgetSelected = delegate { };
    public static event Action<AlarmPriority, AlarmSourceType> OnNotification = delegate { };
    
    internal static void Trigger_PlayerPositionWrapped()
    {
        Debug.Log($"[OuterSpaceEvent] PlayerPositionWrapped");
        PlayerPositionWrapped();
    }
    
    internal static void Trigger_PlayerEnterPlanet()
    {
        Debug.Log($"[OuterSpaceEvent] PlayerEnterPlanet");
        PlayerEnterPlanet();
    }
    
    internal static void Trigger_PlayerRefusePlanet()
    {
        Debug.Log($"[OuterSpaceEvent] PlayerRefusePlanet");
        PlayerRefusePlanet();
    }
    
    public static void Trigger_GadgetSelected(GadgetInfo gadgetInfo)
    {
        Debug.Log($"[OuterSpaceEvent] GadgetSelected");
        OnGadgetSelected(gadgetInfo);
    }

    public static void Trigger_Notification(AlarmPriority alarmPriority, AlarmSourceType objType)
    {
        Debug.Log($"[OuterSpaceEvent] AlarmTriggered");
        OnNotification(alarmPriority, objType);
    }

    public static void Trigger_Boost(bool isOn)
    {
        Debug.Log($"[OuterSpaceEvent] OnBoost {isOn}");
        OnBoost(isOn);
    }
}