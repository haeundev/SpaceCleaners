using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spacecraft : MonoBehaviour
{
    [SerializeField] private List<GameObject> models;
    
    
    
    public void Upgrade(int level)
    {
        models.ForEach(p => p.SetActive(false));
        if (level >= models.Count || level < 0)
        {
            Debug.LogError($"Check spacecraft level.");
            models[0].SetActive(true);
            return;
        }
        models[level].SetActive(true);
    }
}