using System;

public static class OuterSpaceEvent
{
    public static event Action PlayerPositionWrapped = delegate { };
    public static event Action PlayerEnterPlanet = delegate { };
    public static event Action PlayerRefusePlanet = delegate { };
    
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
    
}