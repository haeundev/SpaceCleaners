using System.Collections.Generic;
using UnityEngine;

public class ToggleOthersOnEnableDisable : MonoBehaviour
{
    [SerializeField] private List<GameObject> toggleOnOnEnable;
    [SerializeField] private List<GameObject> toggleOffOnEnable;

    private void OnEnable()
    {
        toggleOnOnEnable.ForEach(p =>
        {
            if (p != null)
                p.SetActive(true);
        });
        toggleOffOnEnable.ForEach(p =>
        {
            if (p != null)
                p.SetActive(false);
        });
    }

    private void OnDisable()
    {
        toggleOnOnEnable.ForEach(p =>
        {
            if (p != null)
                p.SetActive(false);
        });
        
        toggleOffOnEnable.ForEach(p =>
        {
            if (p != null)
                p.SetActive(true);
        });
    }
}