using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GRASBOCK.XR.Inventory;
using UnityEngine.AddressableAssets;

public class KeySpawner : MonoBehaviour
{
    public DynamicQuickAccessInventory _inventory;
    public int maxCount = 1;
    public Transform keySpawnPoint;
    [SerializeField] private string keyPath = "Prefabs/Jungle/JungleKey.prefab";

    // Start is called before the first frame update
    void Awake()
    {
        _inventory.OnKeySpawned += OnKeySpawned;
    }

    public void OnKeySpawned(HashSet<Slot> slots)
    {
        foreach(Slot s in slots)
        {
            if(s.ItemCount < maxCount)
            {
                return;
            }
        }

        //Key spawn
        Addressables.LoadAssetAsync<GameObject>(keyPath).Completed += op =>
        {
            var go = op.Result;
            var temp = Instantiate(go, keySpawnPoint);
            // temp.GetComponent<Floater>().enabled = true;
        };
    }
}
