using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MonsterSpawnPositions : MonoBehaviour
{
    public List<Transform> positions = new();
    
    private void Awake()
    {
        CollectChildrenPosition();
    }

    [Button]
    private void CollectChildrenPosition()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            positions.Add(transform.GetChild(i).transform);
        }
    }
}