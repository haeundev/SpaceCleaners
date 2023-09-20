using GRASBOCK.XR.Inventory;
using LiveLarson.SoundSystem;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InsertItemToSlot : MonoBehaviour
{
    private Slot _slot;
    public bool isInserted;
    private JungleCompletionChecker _checker;

    private void Awake()
    {
        _slot = GetComponent<Slot>();
        _checker = FindObjectOfType<JungleCompletionChecker>();
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        _slot.OnSlotCollision += OnSlotCollision;
    }

    private void OnSlotCollision(Collider collidedItem)
    {
        collidedItem.GetComponentInChildren<XRGrabInteractable>().enabled = false;
        isInserted = true;
        _checker.CheckIfComplete();
        _slot.ItemInfo = collidedItem.GetComponentInChildren<Item>().itemInfo;
        JungleEvents.Trigger_InventoryUpdated(_slot);
        SoundService.PlaySfx("Assets/Audio/UI OK.ogg", transform.position);
    }
}