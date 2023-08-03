using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GRASBOCK.XR.Inventory
{
    public class Inventory : MonoBehaviour
    {
        public HashSet<Slot> slots = new HashSet<Slot>();
        public bool start_closed = true;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            //get all child objects with Slot component
            UpdateSlots();
            if(start_closed)
                Close(); //to prevent the inventory staying open at startup
        }

        public virtual Slot FindSlotWithItem(ItemInfo itemInfo)
        {
            foreach (Slot s in slots)
            {
                if (s.ItemInfo == itemInfo)
                    return s;
            }
            return null;
        }

        public virtual void UpdateSlots()
        {
            //get all child objects with inventorySlot component
            slots = new HashSet<Slot>(GetComponentsInChildren<Slot>());
        }

        public virtual void Open()
        {
            foreach (Slot s in slots)
            {
                s.Open();
            }
        }
        
        // TODO: 지승 확인! 인벤토리 전체에서 이 아이템 갯수를 리턴해야 하는 함수.
        public int GetItemCount(ItemInfo itemInfo)
        {
            var count = 0;
            foreach (var slot in slots)
            {
                if (slot.ItemInfo == itemInfo)
                {
                    count++;
                }
            }
            return count;
        }

        public virtual void Close()
        {
            foreach (Slot s in slots)
                s.Close();
        }
    }
}