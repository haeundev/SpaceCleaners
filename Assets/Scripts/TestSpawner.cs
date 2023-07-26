using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestSpawner : MonoBehaviour
{
    [SerializeField] public AssetReference assetRef;
    
    [Button]
    public void Spawn()
    {
        assetRef.InstantiateAsync(transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Spawn();
        }
    }
}