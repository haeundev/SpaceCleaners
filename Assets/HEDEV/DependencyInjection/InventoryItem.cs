using System;

namespace Proto.DTO
{
    [Serializable]
    public class InventoryItem
    {
        public int itemId;
        public string name;
        public int count;
        public bool newItem;
    }
}