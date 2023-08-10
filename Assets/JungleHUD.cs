using System.Collections.Generic;
using System.Linq;
using GRASBOCK.XR.Inventory;
using UnityEngine;
using DevFeatures.SaveSystem;
using DevFeatures.SaveSystem.Model;

public class JungleHUD : MonoBehaviour
{
    // private DynamicQuickAccessInventory _inventory;
    public DynamicQuickAccessInventory _inventory;
    private List<JungleItemStatusUI> _itemStatusUIs;
    [SerializeField] private List<ItemInfo> itemInfos;

    private PlayerStat _playerStat;

    private void Awake()
    {
        _itemStatusUIs = GetComponentsInChildren<JungleItemStatusUI>().ToList();
        // _inventory = FindObjectOfType<DynamicQuickAccessInventory>();
        _inventory.OnInventoryUpdated += OnInventoryUpdated;

        _playerStat = SaveAndLoadManager.Instance.PlayerStat;
    }

    public void OnOxygenLevelUpdated()
    {

    }

    public void OnInventoryUpdated(HashSet<Slot> slots)
    {
        // TODO: 지승! 인벤토리에서 넘겨준 정보대로 디스플레이 하는 부분.
        foreach (var itemInfo in itemInfos) // leaf, redflower, dasf
        {
            var count = _inventory.GetItemCount(itemInfo);
            _itemStatusUIs.Single(p => p.itemInfo == itemInfo).SetCount(count);
        }
    }
}