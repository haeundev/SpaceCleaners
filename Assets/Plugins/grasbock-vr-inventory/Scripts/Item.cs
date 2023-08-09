using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRASBOCK.XR.Inventory
{
    [RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour
    {
        public ItemInfo itemInfo = null;

        //rigidbody
        Rigidbody rb = null;
        private Slot m_slot = null;
        public Slot Slot
        {
            get { return m_slot; }
            set
            {
                Slot slot_before = m_slot;
                m_slot = value;
                if (value == null)
                {
                    // tell all OTHER touched slots, that they should start attracting the item
                    foreach (Slot slot in touched_slots)
                        if (slot != slot_before)
                            slot.Attract(this, GetComponent<Collider>());
                }
                else
                {
                    foreach (Slot slot in touched_slots)
                        if (slot != value)
                            // other slots shouldn't attract this item, as it now belongs to one specifically
                            slot.StopAttract(this);
                }
                    
            }
        }
        public HashSet<Grabber> grabbers = new HashSet<Grabber>();
        public HashSet<Slot> touched_slots = new HashSet<Slot>();
        // Start is called before the first frame update
        void Start()
        {
            if (!itemInfo) Debug.LogError("There is no ItemInfo attached to the Item component. Without it one will not be able to pull it out or store in an inventory.");
            if (!rb) rb = gameObject.GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Grabber grabber = other.GetComponent<Grabber>();
            if (grabber)
            {
                //Debug.Log("Item is probably being grabbed");
                grabbers.Add(grabber);
                return;
            }

            Slot slot = other.GetComponent<Slot>();
            if (slot)
            {
                touched_slots.Add(slot);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Grabber grabber = other.GetComponent<Grabber>();
            if (grabber)
            {
                //Debug.Log("One less grabber on item");
                grabbers.Remove(grabber);
                return;
            }

            Slot slot = other.GetComponent<Slot>();
            if (slot)
            {
                touched_slots.Remove(slot);
            }
        }

        private void OnDestroy()
        {
            // OnTriggerExit is not called when the Item gets destroyed
            foreach(Grabber grabber in grabbers)
            {
                grabber.touched_items.Remove(this);
            }
            foreach (Slot s in touched_slots)
            {
                s.StopAttract(this);
            }

        }
    }
}