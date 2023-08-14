using System;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class FuelCountPopupUI : MonoBehaviour
{
    public static FuelCountPopupUI Instance;
    private TextMeshProUGUI _tmp;
    private int _fuelCount;
    private Vector3 _originalScale;
    private Vector3 _scaleTo;

    private void Awake()
    {
        Instance = this;
        Instance.gameObject.SetActive(false);
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    public static void ShowIncrease()
    {
        Instance._fuelCount++;
        Instance._tmp.text = $"{Instance._fuelCount} / 10";
        Instance.gameObject.SetActive(true);
        if (Instance._fuelCount == 10)
        {
            Instance.SpawnKey();
        }
        Observable.Timer(TimeSpan.FromSeconds(3f)).Subscribe(_ => Instance.gameObject.SetActive(false));
    }

    [Button]
    private void SpawnKey()
    {
        Addressables.InstantiateAsync("Prefabs/Jungle/JungleKey.prefab", transform.position, Quaternion.identity);
    }
}