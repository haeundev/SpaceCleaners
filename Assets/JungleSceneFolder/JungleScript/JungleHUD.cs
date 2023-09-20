using System.Collections.Generic;
using System.Linq;
using DevFeatures.SaveSystem.Model;
using GRASBOCK.XR.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class JungleHUD : MonoBehaviour
{
    // private DynamicQuickAccessInventory _inventory;
    public DynamicQuickAccessInventory _inventory;
    private List<JungleItemStatusUI> _itemStatusUIs;
    [SerializeField] private List<ItemInfo> itemInfos;

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    private PlayerStat _playerStat;
    public int maxOxygen = 100;
    public int currentOxygen;

    private void Awake()
    {
        _itemStatusUIs = GetComponentsInChildren<JungleItemStatusUI>().ToList();
        // _inventory = FindObjectOfType<DynamicQuickAccessInventory>();
        // _inventory.OnInventoryUpdated += OnInventoryUpdated;

        // _playerStat = SaveAndLoadManager.Instance.PlayerStat;
    }

    private void Start()
    {
        //_playerStat = SaveAndLoadManager.Instance.PlayerStat;
        // print(_playerStat.oxygenLevel);
        // SetOxygenLevel(_playerStat.oxygenLevel);

        SetOxygenLevel(0);
    }

    public void SetOxygenLevel(int oxygenValue)
    {
        currentOxygen += oxygenValue;
        slider.value = currentOxygen;
        // fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void OnOxygenLevelUpdated() //oxygen 획득하면
    {
        if (currentOxygen < maxOxygen) SetOxygenLevel(20);
    }

    // public void OnInventoryUpdated(HashSet<Slot> slots)
    // {
    //     // TODO: 지승! 인벤토리에서 넘겨준 정보대로 디스플레이 하는 부분.
    //     foreach (var itemInfo in itemInfos) // leaf, redflower, dasf
    //     {
    //         var count = _inventory.GetItemCount(itemInfo);
    //         _itemStatusUIs.Single(p => p.itemInfo == itemInfo).SetCount(count);
    //     }
    // }
}