using System;
using UnityEngine;

public static class JungleEvents
{
    public static event Action<GameObject> OnPlantGrowDone = delegate { };
    
    public static void Trigger_PlantGrowDone(GameObject plantObj)
    {
        Debug.Log($"[JungleEvents] Plant Grow Done");
        OnPlantGrowDone(plantObj);
    }
}