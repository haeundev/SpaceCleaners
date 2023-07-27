using System.Collections.Generic;
using LiveLarson.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ItemRandomSpawner : MonoBehaviour
{
    [SerializeField] private List<AssetReference> itemRefs;

    private void Awake()
    {
        GetComponentInChildren<MeshRenderer>().enabled = false;
        
        var rnd = Random.Range(0, 10);
        if (rnd < 5)
        {
            Addressables.InstantiateAsync(itemRefs.PeekRandom(), transform);
        }
    }
}