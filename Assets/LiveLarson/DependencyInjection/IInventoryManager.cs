using System.Collections.Generic;
using Enums;
using UnityEngine.Events;

namespace LiveLarson.DependencyInjection
{
    public interface IInventoryManager
    {
        public enum AcquireUIType
        {
            None,
            ToastUI,
        }

        UnityEvent<InventoryItem> ItemAdded { get; }
        UnityEvent<int, int> ItemUsed { get; }
        UnityEvent<List<InventoryItem>> ItemLoaded { get; }

        void AddItem(int id);
        void AddItem(int id, int count);
        void AddItem(int id, int count, AcquireUIType acquireUIType);
        void AddItem(Item item);
        void AddItem(Item item, AcquireUIType acquireUIType);
        InventoryItem FindItem(int itemId);
        List<InventoryItem> GetAllItems();
        List<int> GetAllItemIds(ItemType type);
        IEnumerable<InventoryItem> GetAllItems(ItemType type);
        bool HasItem(int itemID, int count = 1);
        int GetItemCount(int itemID);
        void LoadData();
    }
}