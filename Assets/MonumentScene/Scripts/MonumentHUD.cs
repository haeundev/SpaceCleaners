using System.Collections.Generic;
using System.Linq;
using GRASBOCK.XR.Inventory;
using UnityEngine;
using DevFeatures.SaveSystem;
using DevFeatures.SaveSystem.Model;
using UnityEngine.UI;

public class MonumentHUD : MonoBehaviour
{
    public CheckPlayerCollision _playerCollision;
    // private DynamicQuickAccessInventory _inventory;
    public DynamicQuickAccessInventory _inventory;
    private List<JungleItemStatusUI> _itemStatusUIs;
    [SerializeField] private List<ItemInfo> itemInfos;

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    private PlayerStat _playerStat;

    public int maxHealth = 100;
    public int currentHealth = 100;

    private void Awake()
    {
        _itemStatusUIs = GetComponentsInChildren<JungleItemStatusUI>().ToList();
        // _inventory = FindObjectOfType<DynamicQuickAccessInventory>();
        _inventory.OnInventoryUpdated += OnInventoryUpdated;
        _playerCollision.OnPlayerDamaged += TakeDamage;

        // _playerStat = SaveAndLoadManager.Instance.PlayerStat;
    }

    void Start()
    {
        _playerStat = SaveAndLoadManager.Instance.PlayerStat;
        // print(_playerStat.oxygenLevel);
        // SetOxygenLevel(_playerStat.oxygenLevel);

        TakeDamage(0);
    }

    public void TakeDamage(int value)
    {
        currentHealth -= value;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);

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