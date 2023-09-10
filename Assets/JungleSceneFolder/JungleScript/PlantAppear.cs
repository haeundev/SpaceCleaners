using System;
using System.Collections;
using System.Collections.Generic;
using LiveLarson.Util;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlantAppear : MonoBehaviour
{
    public GameObject volumeObj;

    private ColorAdjustments _colorAdjustments;

    [SerializeField] private float posY = 2;
    [SerializeField] private float deltaHue = 6f;
    [SerializeField] private float sec = 0.02f;
    private Vector3 _originalPos;

    private void Awake()
    {
        Volume volume = volumeObj.GetComponent<Volume>();
        ColorAdjustments tmp;
        if(volume.profile.TryGet<ColorAdjustments>(out tmp))
        {
            _colorAdjustments = tmp;
        }

        _colorAdjustments.hueShift.value = -60f;
    }

    // Start is called before the first frame update
    void Start()
    {
        _originalPos = new Vector3(0, -60, 0);
        
        Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
        {
            StartCoroutine(JungleFlourish());
        });
    }

    [Button]
    IEnumerator JungleFlourish()
    {
        transform.position = _originalPos;
        
        while (transform.position.y < 0) //_colorAdjustments.hueShift.value < 0
        {
            transform.position += new Vector3(0, posY, 0); //
            _colorAdjustments.hueShift.value += deltaHue;
            yield return YieldInstructionCache.WaitForSeconds(sec); //

        }
    }
}
