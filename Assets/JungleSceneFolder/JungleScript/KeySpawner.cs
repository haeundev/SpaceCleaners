using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GRASBOCK.XR.Inventory;
using LiveLarson.DataTableManagement;
using LiveLarson.SoundSystem;
using UnityEngine.AddressableAssets;

public class KeySpawner : MonoBehaviour
{
    public DynamicQuickAccessInventory _inventory;
    public int maxCount = 5;
    public Transform keySpawnPoint;
    [SerializeField] private string keyPath = "Prefabs/Jungle/JungleKey.prefab";

    [SerializeField] private string keySpawnSFX = "Assets/Audio/key_appear_magic.mp3";
    // [SerializeField] private AudioClip clip;

    // Start is called before the first frame update
    void Awake()
    {
        _inventory.OnKeySpawned += OnKeySpawned;
    }

    public void OnKeySpawned(HashSet<Slot> slots)
    {
        foreach(Slot s in slots)
        {
            if(s.ItemCount != maxCount)
            {
                return;
            }
        }

        //Key spawn
        Addressables.LoadAssetAsync<GameObject>(keyPath).Completed += op =>
        {
            var go = op.Result;
            var temp = Instantiate(go, keySpawnPoint);
            SoundService.PlaySfx(keySpawnSFX, temp.transform.position);
            // AudiosourceManager.instance.PlayClip(clip);
            // temp.GetComponent<Floater>().enabled = true;
        };
    }
}
