using System;
using UnityEngine;

public static class MonumentEvents
{
    public static event Action OnSceneComplete = delegate { };
    public static event Action OnSceneLoaded = delegate { };
    public static event Action OnRecycleCorrect = delegate { };
    public static event Action OnRecycleWrong = delegate { };

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

    internal static void Trigger_RecycleCorrect()
    {
        Debug.Log("[MonumentEvents] RecycleCorrect");
        OnRecycleCorrect();
    }

    internal static void Trigger_RecycleWrong()
    {
        Debug.Log("[MonumentEvents] RecycleWrong");
        OnRecycleWrong();
    }
}