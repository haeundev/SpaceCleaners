using System;
using System.Collections;
using System.Collections.Generic;
using DataTables;
using UnityEngine;

public class SelectGadgetIcon : MonoBehaviour
{
   
    [SerializeField] private GameObject _gadgetIcon;

    private void Awake()
    {
        OuterSpaceEvent.OnGadgetSelected += OnGadgetSelected;
    }

    private void OnGadgetSelected(GadgetInfo _)
    {
        
    }
}
