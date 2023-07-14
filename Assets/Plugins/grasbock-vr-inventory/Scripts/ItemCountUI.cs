using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRASBOCK.XR.Inventory {
    [RequireComponent(typeof(TextMesh))]
    public class ItemCountUI : MonoBehaviour
    {
        TextMesh tm = null;
        public Slot slot;
        ~ItemCountUI()
        {
            if (slot != null)
            {
                slot.item_count_subscribers.Remove(CountChanged);
            }
        }

        /// <summary>
        /// When the Slot that has been subscribed to changes, this function is called to update the counter
        /// </summary>
        public void CountChanged(Slot s)
        {
            // do not display 1 and 0
            if (s.ItemCount < 2) gameObject.SetActive(false);
            else gameObject.SetActive(true);

            // change text and make sure it will not be too many numbers
            if(s.ItemCount > 999)
                tm.text = "+999";
            else
                tm.text = s.ItemCount.ToString();
        
            // set to correct character size (more numbers need more space), so that they fit into a circle
            tm.fontSize = (int)(111.25f - 35.25f*tm.text.Length + 3.75f*tm.text.Length*tm.text.Length); // LSE fit polynomial (1-4 characters)
        }

        // Start is called before the first frame update
        void Start()
        {
            tm = GetComponentInChildren<TextMesh>();

            if (!slot) slot = gameObject.GetComponentInParent<Slot>();
            if (!slot) Debug.LogWarning("No Slot could be found to display the item count");
            else
            {
                slot.item_count_subscribers.Add(CountChanged);
                CountChanged(slot); // first update
            }
        }
    }
}