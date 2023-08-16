using System.Collections.Generic;
using GRASBOCK.XR.Inventory;
using LiveLarson.SoundSystem;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    public DynamicQuickAccessInventory _inventory;
    [SerializeField] private int maxCount;
    public Transform keySpawnPoint;
    [SerializeField] private GameObject keyPrefab;

    [SerializeField] private string keySpawnSFX = "Assets/Audio/key_appear_magic.mp3";

    private bool isKeySpawned;

    private void Awake()
    {
        _inventory.OnKeySpawned += OnKeySpawned;
    }

    public void OnKeySpawned(HashSet<Slot> slots)
    {
        if (isKeySpawned)
            return;

        foreach (var s in slots)
            if (s.ItemCount != maxCount)
                return;

        var key = Instantiate(keyPrefab, keySpawnPoint);
        SoundService.PlaySfx(keySpawnSFX, key.transform.position);
        isKeySpawned = true;
    }
}