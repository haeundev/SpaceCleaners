using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LiveLarson.DataTableManagement;
using LiveLarson.SoundSystem;
using UnityEngine.XR.Content.Interaction;

public class OnChestOpened : MonoBehaviour
{
    private OnTrigger _onTrigger;
    [SerializeField] private string chestBoxSFX = "Assets/Audio/loot-box.mp3";
    // Start is called before the first frame update

    private void Awake()
    {
        _onTrigger = GetComponent<OnTrigger>();
        RegisterEvents();
        
    }
    
    private void RegisterEvents()
    {
        _onTrigger.OnChestOpened += OnChestboxOpened;
    }
    
    private void OnChestboxOpened()
    {
        SoundService.PlaySfx(chestBoxSFX, transform.position);
    }
    
    
}
