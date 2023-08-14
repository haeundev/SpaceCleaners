using System;
using UnityEngine;

public static class JungleEvents
{
    public static event Action<GameObject> OnPlantGrowDone = delegate { };
    public static event Action OnSceneLoaded = delegate { };
    public static event Action OnSceneComplete = delegate { };
    
    public static void Trigger_PlantGrowDone(GameObject plantObj)
    {
        Debug.Log($"[JungleEvents] Plant Grow Done");
        OnPlantGrowDone(plantObj);
    }
    
    internal static void Trigger_SceneComplete()
    {
        Debug.Log("[JungleEvents] OnSceneComplete");
        OnSceneComplete();
    }
    
    internal static void Trigger_SceneLoaded()
    {
        Debug.Log("[JungleEvents] OnSceneLoaded");
        OnSceneLoaded();
    }
}