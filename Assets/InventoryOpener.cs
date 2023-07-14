using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryOpener : MonoBehaviour
{
    [SerializeField] private InputActionProperty button;
    [SerializeField] private GameObject bagObject;
    private bool _isOpen;
    
    private void Update()
    {
        if (button.action.WasPressedThisFrame())
        {
            OpenInventory(!_isOpen);
            _isOpen = !_isOpen;
        }
    }

    private void OpenInventory(bool isOpen)
    {
        bagObject.SetActive(isOpen);
    }
}