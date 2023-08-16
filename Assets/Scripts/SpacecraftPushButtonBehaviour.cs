using System;
using DataTables;
using LiveLarson.Enums;
using LiveLarson.SoundSystem;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SpacecraftPushButtonBehaviour : MonoBehaviour
{
    private Transform _playerTransform;
    private SpacePlayer _spacePlayer;
    private XRPushButton _pushButton;
    
    private float _wheelValue; // between -1 ~ 1
    private bool _isWheelActivated;
    private GadgetInfo _currentGadget;

    [SerializeField] private Grappler grappler;

    private void Awake()
    {
        _pushButton = gameObject.GetComponent<XRPushButton>();
        var playerObj = GameObject.FindWithTag("Player");
        _playerTransform = playerObj.transform;
        _spacePlayer = playerObj.GetComponent<SpacePlayer>();
        
        _pushButton.onPress.AddListener(OnPressed);
        _pushButton.onRelease.AddListener(OnReleased);
        OuterSpaceEvent.OnGadgetSelected += OnGadgetSelected;
    }

    private void OnDestroy()
    {
        OuterSpaceEvent.OnGadgetSelected -= OnGadgetSelected;
    }

    private void OnGadgetSelected(GadgetInfo gadgetInfo)
    {
        _currentGadget = gadgetInfo;
    }

    private void OnPressed()
    {
        if (_currentGadget == default)
        {
            Debug.LogError($"No gadget selected.");
            return;
        }
        
        switch (_currentGadget.GadgetType)
        {
            case GadgetType.Harpoon:
                grappler.DoGrapple();
                SoundService.PlaySfx("Audio/SFX/Flare gun 5-2.wav", transform.position);
                break;
            case GadgetType.Net:
                break;
            case GadgetType.Laser:
                break;
        }
    }
    
    private void OnReleased()
    {
        if (_currentGadget == default)
        {
            Debug.LogError($"No gadget selected.");
            return;
        }
        
        switch (_currentGadget.GadgetType)
        {
            case GadgetType.Harpoon:
                grappler.OnGrappleDone();
                break;
            case GadgetType.Net:
                break;
            case GadgetType.Laser:
                break;
        }
    }
}