using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRASBOCK.XR.Inventory
{
    [RequireComponent(typeof(Collider))]
    public class Grabber : MonoBehaviour
    {
        public HashSet<Item> touched_items = new HashSet<Item>();

        private void OnTriggerEnter(Collider other)
        {
            Item item = other.GetComponent<Item>();
            if(item)
            {
                touched_items.Add(item);
            } 
            
        }

        private void OnTriggerExit(Collider other)
        {
            Item item = other.GetComponent<Item>();
            if(item)
            {
                touched_items.Remove(item);
            } 
        }
    }
}

