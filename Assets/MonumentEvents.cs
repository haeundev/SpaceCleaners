using System;
using UnityEngine;

public static class MonumentEvents
{
    public static event Action OnSceneComplete = delegate { };

    internal static void Trigger_SceneComplete()
    {
        Debug.Log("[MonumentEvents] OnSceneComplete");
        OnSceneComplete();
    }
}