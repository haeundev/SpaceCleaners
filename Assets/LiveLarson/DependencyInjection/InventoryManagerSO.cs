using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace LiveLarson.DependencyInjection
{
    public class InventoryManagerSO : ScriptableObject
    {
        [Serializable]
        public class InventoryData
        {
            public List<InventoryItem> inventoryItems;
        }

        public UnityEvent<InventoryItem> ItemAdded { get; } = new();
        public UnityEvent<int, int> ItemUsed { get; } = new();
        public UnityEvent<List<InventoryItem>> ItemLoaded { get; } = new();

        [SerializeField] private List<InventoryItem> inventoryList = new();
        [Inject] private InventorySaveService _saveService;

        public static Item FindItemData(int itemID)
        {
            return default;
            // return DataTableManager.Items.Find(itemID);
        }

        public void AddItem(int id)
        {
            AddItem(id, 1, IInventoryManager.AcquireUIType.ToastUI);
        }

        public void AddItem(int id, int count)
        {
            AddItem(id, count, IInventoryManager.AcquireUIType.ToastUI);
        }

        public void AddItem(int id, int count, IInventoryManager.AcquireUIType acquireUIType)
        {
            if (id == 0 || count < 1)
                return;
            if (TryGetItemFromId(id, out var itemData))
            {
                // TODO: implement
            }
        }

        public void AddItem(Item item)
        {
            throw new NotImplementedException();
        }

        public void AddItem(Item item, IInventoryManager.AcquireUIType acquireUIType)
        {
            throw new NotImplementedException();
        }

        private bool TryGetItemFromId(int itemId, out Item item)
        {
            item = FindItemData(itemId);
            if (item != default)
                return true;
            Debug.LogError($"{itemId}라는 존재하지 않는 아이템을 추가하려 했습니다.");
            return false;
        }

        public InventoryItem FindItem(int itemId)
        {
            var findItem = inventoryList.Find(searchItem => searchItem.itemId == itemId);
            return findItem;
        }

        public List<InventoryItem> GetAllItems()
        {
            return inventoryList;
        }

        // public List<int> GetAllItemIds(ItemType type)
        // {
        //     return inventoryList.Select(p => FindItemData(p.itemId)).Where(p => p.ItemType == type)
        //         .Select(p => p.ID).ToList();
        // }

        // public IEnumerable<InventoryItem> GetAllItems(ItemType type)
        // {
        //     return inventoryList.Where(p =>
        //     {
        //         var itemData = FindItemData(p.itemId);
        //         if (itemData == default)
        //             return false;
        //         return itemData.ItemType == type;
        //     });
        // }

        public bool HasItem(int itemID, int count = 1)
        {
            var itemFound = inventoryList?.Find(item => item.itemId == itemID);
            if (itemFound == null)
                return false;

            return itemFound.count >= count;
        }

        public int GetItemCount(int itemID)
        {
            if (itemID <= 0)
                return 0;

            var itemFound = inventoryList.Find(item => item.itemId == itemID);
            return itemFound?.count ?? 0;
        }
        
        // public static string GetIconPath(int itemID)
        // {
        //     var item = FindItemData(itemID);
        //     return item != default ? $"{item.Icon}.png" : string.Empty;
        // }
        
        public void LoadData() // 이후 서버 작업 시 로그인 데이터를 이용하여 로드 하도록 변경 필요
        {
            // inventoryList.Clear();
            // PostLoginService.Instance.LoadData(dto =>
            // {
            //     var inventoryItems = new List<InventoryItem>();
            //     if (dto is PostLoginRespDTO post)
            //     {
            //         inventoryItems.AddRange(post.student.inventory);
            //         SetInventory(inventoryItems);
            //         ItemLoaded?.Invoke(inventoryList);
            //     }
            //     else
            //     {
            //         if (dto is InventoryData inventoryData)
            //         {
            //             SetInventory(inventoryData.inventoryItems);
            //             ItemLoaded?.Invoke(inventoryList);
            //         }
            //     }
            // }, null);
        }

        private void SetInventory(List<InventoryItem> inventoryData)
        {
            if (inventoryData == null)
            {
                Debug.LogWarning("Inventory Data is null - not setting dictionary");
                return;
            }

            foreach (var data in inventoryData)
            {
                var itemFound = inventoryList.Find(itemListed => itemListed.itemId == data.itemId);
                if (itemFound != null)
                    itemFound.count = data.count;
                else
                    inventoryList.Add(data);
            }
        }
    }
}