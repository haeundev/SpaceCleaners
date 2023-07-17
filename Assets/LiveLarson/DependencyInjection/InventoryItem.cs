using System;

namespace LiveLarson.DependencyInjection
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