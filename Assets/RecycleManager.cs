using System.Collections.Generic;
using UnityEngine;

public class RecycleManager : MonoBehaviour
{
    public static RecycleManager Instance { get; private set; }
    public List<RecycleBox> recycleBoxes = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterRecycleBox(RecycleBox recycleBox)
    {
        recycleBoxes.Add(recycleBox);
    }
    
    public void CheckIfAllBoxesDone()
    {
        if (recycleBoxes.TrueForAll(box => box.IsDone))
        {
            MonumentEvents.Trigger_SceneComplete();
        }
    }

    public void OnOneBoxDone()
    {
        CheckIfAllBoxesDone();
    }
}