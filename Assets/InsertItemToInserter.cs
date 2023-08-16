using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GRASBOCK.XR.Inventory;
using LiveLarson.DataTableManagement;
using LiveLarson.SoundSystem;

public class InsertItemToInserter : MonoBehaviour
{
    private Inserter _inserter;
    // Start is called before the first frame update

    private void Awake()
    {
        _inserter = FindObjectOfType<Inserter>();
        RegisterEvents();
    }
    
    private void RegisterEvents()
    {
        _inserter.OnItemInserted += OnItemInserted;
    }
    
    private void OnItemInserted()
    {
        SoundService.PlaySfx("Assets/Audio/insert_craft.mp3", transform.position);
    }
}
