using GRASBOCK.XR.Inventory;
using TMPro;
using UnityEngine;

public class JungleItemStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] public ItemInfo itemInfo;

    private void Awake()
    {
        JungleEvents.OnInventoryUpdated += OnInventoryUpdated;
    }

    private void OnInventoryUpdated(Slot slot)
    {
        if (slot.ItemInfo == itemInfo)
            text.text = "1/1";
    }
}