using GRASBOCK.XR.Inventory;
using LiveLarson.SoundSystem;
using UnityEngine;

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
        SoundService.PlaySfx("Assets/Audio/UI OK.ogg", transform.position);
    }
}