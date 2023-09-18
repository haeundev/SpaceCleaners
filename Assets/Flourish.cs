using System;
using System.Collections;
using System.Collections.Generic;
using LiveLarson.SoundSystem;
using LiveLarson.Util;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Flourish : MonoBehaviour
{
    private int currentWateredCount;
    private int treeCount;
    
    private float deltaHue;

    [SerializeField] private Volume vol;
    private ColorAdjustments _colorAdjustments;
    
    // Start is called before the first frame update
    private void Awake()
    {
        Volume volume = vol.GetComponent<Volume>();
        ColorAdjustments tmp;
        if(volume.profile.TryGet<ColorAdjustments>(out tmp))
        {
            _colorAdjustments = tmp;
        }
        
        _colorAdjustments.hueShift.value = -60f;
        
        GameObject[] trees = GameObject.FindGameObjectsWithTag("JungleTree");
        treeCount = trees.Length;
        print("tree Count: "+treeCount);
        
        
        deltaHue = 60f / treeCount;
        
        JungleEvents.OnPlantGrowDone += OnPlantGrowDone;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnPlantGrowDone(GameObject plantObj)
    {
        currentWateredCount++;
        float temp = _colorAdjustments.hueShift.value + deltaHue;
        StartCoroutine(JungleFlourish(temp));

        if (currentWateredCount == treeCount)
        {
            
            Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(_ =>
            {
                SoundService.PlaySfx("Assets/Audio/chestbox_open_magic.mp3", transform.position);
                StartCoroutine(JunglePlantArise());
            });
            
        }
        
    }

    IEnumerator JunglePlantArise()
    {
        while (transform.position.y < 0) //_colorAdjustments.hueShift.value < 0
        {
            // transform.position += new Vector3(0, 2, 0); //
            // yield return YieldInstructionCache.WaitForSeconds(0.02f); //
            
            transform.position += new Vector3(0, 1.5f, 0); //
            yield return YieldInstructionCache.WaitForSeconds(0.085f); //

        }
    }
    
    IEnumerator JungleFlourish(float dest)
    {
        while (_colorAdjustments.hueShift.value < dest)
        {
            _colorAdjustments.hueShift.value = Mathf.Lerp(_colorAdjustments.hueShift.value, dest, 0.5f);
            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }
        
    }
}
