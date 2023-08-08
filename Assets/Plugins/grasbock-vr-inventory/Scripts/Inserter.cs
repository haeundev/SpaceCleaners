using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRASBOCK.XR.Inventory
{
    /// <summary>
    /// Checks for items that touch it and stores them if a Slot can be found. <br>
    /// Needs a delegate function <c>target</c> to call when an Item touches the Inserter
    /// </summary>
    public class Inserter : MonoBehaviour
    {

        public delegate Slot notifyIncomingItem(Inserter inserter, Item i);
        public notifyIncomingItem target = null;

        
        private void OnTriggerStay(Collider other)
        {
            Item item = other.GetComponent<Item>();
            if (!item)
                //its not an item
                return;
            GameObject go = item.gameObject;
            if (go.transform.parent != null)
                //this item is attached to something, so insertion was probably not intended
                return;
            if (item.Slot) 
                // the item already belongs to a slot
                return;

            Slot slot = target?.Invoke(this, item);
            if (!slot)
                //did't find a slot for this item
                return;

            uint store_count = slot.StoreItems(1, item.itemInfo);
            
            if(store_count == 1)
            {
                Destroy(item.gameObject);
            }
        }
    }
}