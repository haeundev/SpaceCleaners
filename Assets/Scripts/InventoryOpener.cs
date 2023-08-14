using UnityEngine;
using UnityEngine.InputSystem;
using GRASBOCK.XR.Inventory;
using LiveLarson.SoundSystem;

public class InventoryOpener : MonoBehaviour
{
    [SerializeField] private InputActionProperty button;
    [SerializeField] private GameObject bagObject;
    private bool _isOpen;
    
    [SerializeField] private AudioClip clip;
    // private string inventoryOpenSFX = "Assets/Audio/MouseClick.mp3";
    
    private void Update()
    {
        if (button.action.WasPressedThisFrame())
        {
            OpenInventory(!_isOpen);
            // _audioSource.clip = clip;
            // _audioSource.Play();
            AudiosourceManager.instance.PlayClip(clip);
            // SoundService.PlaySfx(inventoryOpenSFX, transform.position);
            _isOpen = !_isOpen;
        }
    }

    private void OpenInventory(bool isOpen)
    {
        bagObject.SetActive(isOpen);
    }
}