using System.Collections.Generic;
using UnityEngine;

public class SpaceObjectLabels : MonoBehaviour
{
    public static SpaceObjectLabels Instance;
    public List<SpaceObjectLabel> labels;

    private void Awake()
    {
        Instance = this;
        // labels = new List<SpaceObjectLabel>(GetComponentsInChildren<SpaceObjectLabel>());
    }

    public static void DisableAll()
    {
        Instance.labels.ForEach(p => p.gameObject.SetActive(false));
    }
}