using UnityEngine;

public class JungleSceneLoadedChecker : SceneLoadedChecker
{
    protected override void OnComplete()
    {
        Debug.Log("All GameObjects are not null");
        JungleEvents.Trigger_SceneLoaded();
        Destroy(gameObject);
    }
}