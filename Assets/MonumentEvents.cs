using System;
using UnityEngine;

public static class MonumentEvents
{
    public static event Action OnSceneComplete = delegate { };
    public static event Action OnSceneLoaded = delegate { };

    internal static void Trigger_SceneComplete()
    {
        Debug.Log("[MonumentEvents] OnSceneComplete");
        OnSceneComplete();
    }

    internal static void Trigger_SceneLoaded()
    {
        Debug.Log("[MonumentEvents] OnSceneLoaded");
        OnSceneLoaded();
    }
}