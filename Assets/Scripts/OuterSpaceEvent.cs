using System;

public static class OuterSpaceEvent
{
    public static event Action PlayerPositionWrapped = delegate { };
    public static event Action PlayerEnterPlanet = delegate { };
    public static event Action PlayerRefusePlanet = delegate { };
    public static event Action<NotificationType> NotificationReceived = delegate { };
    
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
    
}