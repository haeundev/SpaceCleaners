using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRASBOCK.XR.Inventory
{
    public class ItemPreview : MonoBehaviour
    {
        public delegate void OnTouch(Grabber grabber);
        public OnTouch on_touch = null;

        private void OnTriggerEnter(Collider other)
        {
            Grabber grabber = other.GetComponent<Grabber>();
            if (grabber != null && on_touch != null) on_touch(grabber);
        }
    }

}
