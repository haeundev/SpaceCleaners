using UnityEngine;

public class MonumentSceneLoadedChecker : SceneLoadedChecker
{
    protected override void OnComplete()
    {
        Debug.Log("All GameObjects are not null");
        MonumentEvents.Trigger_SceneLoaded();
        Destroy(this);
    }
}