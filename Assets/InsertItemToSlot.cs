using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GRASBOCK.XR.Inventory;
using LiveLarson.DataTableManagement;
using LiveLarson.SoundSystem;

public class InsertItemToSlot : MonoBehaviour
{
    private Slot _slot;
    // Start is called before the first frame update

    private void Awake()
    {
        _slot = GetComponent<Slot>();
        RegisterEvents();
    }
    
    private void RegisterEvents()
    {
        _slot.OnSlotCollision += OnSlotCollision;
    }
    
    private void OnSlotCollision()
    {
        SoundService.PlaySfx(DataTableManager.GameConst.Data.SlotCollision, transform.position);
    }
}
