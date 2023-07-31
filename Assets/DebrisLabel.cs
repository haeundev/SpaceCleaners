using System.Collections.Generic;
using UnityEngine;

public class DebrisLabel : MonoBehaviour
{
    private Gadget _gadget;
    private GameObject _debrisObj;
    [SerializeField] private float requiredDistance = 30f;
    [SerializeField] private List<GameObject> showIfNoGadget;
    [SerializeField] private List<GameObject> showIfTooFar;
    [SerializeField] private List<GameObject> showIfCaptureReady;

    private void Start()
    {
        HideAll();
    }

    private void HideAll()
    {
        showIfNoGadget.ForEach(p => p.SetActive(false));
        showIfTooFar.ForEach(p => p.SetActive(false));
        showIfCaptureReady.ForEach(p => p.SetActive(false));
    }

    private void OnEnable()
    {
        _gadget = FindObjectOfType<Gadget>();
        if (_gadget == default)
        {
            showIfNoGadget.ForEach(p => p.SetActive(true));
            return;
        }

        if (requiredDistance < Vector3.Distance(_gadget.transform.position, _debrisObj.transform.position))
        {
            showIfTooFar.ForEach(p => p.SetActive(true));
            return;
        }
        
        showIfCaptureReady.ForEach(p => p.SetActive(true));
    }
    
    private void OnDisable()
    {
        HideAll();
    }

    public void SetDebris(GameObject debris)
    {
        _debrisObj = debris;
    }
}