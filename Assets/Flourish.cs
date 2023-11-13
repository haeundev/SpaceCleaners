using System;
using System.Collections;
using GRASBOCK.XR.Inventory;
using LiveLarson.SoundSystem;
using LiveLarson.Util;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Flourish : MonoBehaviour
{
    private int _currentWateredCount;
    private int _treeCount;
    private float _deltaHue;

    [SerializeField] private Volume vol;
    private ColorAdjustments _colorAdjustments;
    private Vector3 _initialPos;

    private const int TotalItemCount = 3;
    private int _completedItemCount = 0;

    private void Awake()
    {
        var volume = vol.GetComponent<Volume>();
        ColorAdjustments tmp;
        if (volume.profile.TryGet(out tmp))
            _colorAdjustments = tmp;

        _colorAdjustments.hueShift.value = -60f;

        var trees = GameObject.FindGameObjectsWithTag("JungleTree");
        _treeCount = trees.Length;
        print("tree Count: " + _treeCount);

        _deltaHue = 60f / _treeCount;

        JungleEvents.OnInventoryUpdated += OnInventoryUpdated;

        _initialPos = transform.position;
    }

    private void OnDestroy()
    {
        JungleEvents.OnInventoryUpdated -= OnInventoryUpdated;
    }

    private void OnInventoryUpdated(Slot _)
    {
        _completedItemCount++;

        if (TotalItemCount == _completedItemCount)
        {
            OnAllItemsCollected();
        }
    }

    [Button]
    private void OnAllItemsCollected()
    {
        _currentWateredCount++;
        var temp = _colorAdjustments.hueShift.value + _deltaHue;

        if (_currentWateredCount == _treeCount)
            StartCoroutine(JungleFlourish(temp));
        // SoundService.PlaySfx("Assets/Audio/chestbox_open_magic.mp3", transform.position);
        var sfx = SoundService.PlaySfx("Assets/Audio/GroundRumble.mp3", transform.position);
        StartCoroutine(JunglePlantArise(sfx.Stop));
    }

    [SerializeField] private float riseYBy = 0.1f;
    [SerializeField] private float riseWaitFor = 0.001f;

    private IEnumerator JunglePlantArise(Action onDone)
    {
        transform.position = _initialPos;

        while (transform.position.y < 0)
        {
            transform.position += new Vector3(0, riseYBy, 0);
            yield return YieldInstructionCache.WaitForSeconds(riseWaitFor);
        }
        
        onDone?.Invoke();
    }

    private IEnumerator JungleFlourish(float dest)
    {
        while (_colorAdjustments.hueShift.value < dest)
        {
            _colorAdjustments.hueShift.value = Mathf.Lerp(_colorAdjustments.hueShift.value, dest, 0.5f);
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }
    }
}