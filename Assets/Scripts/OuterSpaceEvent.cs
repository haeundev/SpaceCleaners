using System;
using DataTables;

public static class OuterSpaceEvent
{
    public static event Action PlayerPositionWrapped = delegate { };
    public static event Action PlayerEnterPlanet = delegate { };
    public static event Action PlayerRefusePlanet = delegate { };
    public static event Action<NotificationType> NotificationReceived = delegate { };
    public static event Action<GadgetInfo> GadgetSelected = delegate { };
    
    internal static void Trigger_PlayerPositionWrapped()
    {
        PlayerPositionWrapped();
    }
    
    internal static void Trigger_PlayerEnterPlanet()
    {
        PlayerEnterPlanet();
    }
    
    internal static void Trigger_PlayerRefusePlanet()
    {
        PlayerRefusePlanet();
    }
    
    internal static void Trigger_NotificationReceived(NotificationType notificationType)
    {
        NotificationReceived(notificationType);
    }

    public static void Trigger_GadgetSelected(GadgetInfo gadgetInfo)
    {
        GadgetSelected(gadgetInfo);
    }
}