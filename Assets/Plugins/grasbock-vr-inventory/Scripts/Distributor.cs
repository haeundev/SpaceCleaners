using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GRASBOCK.XR.Inventory
{
    public class Distributor : MonoBehaviour
    {
        public Inserter inserter;
        [Serializable]
        public struct Target
        {
            public ItemInfo itemInfo;
            public Slot slot;
        }
        public Target[] targets;

        // Start is called before the first frame update
        void Start()
        {
            if (!inserter)
            {//the inserter is the point where items will get put into
                Debug.Log("[Distributer] Kind of pointless without an Inserter");
            }
            else
            {
                inserter.target = InsertionIntoInserter;
            }
        }

        /// <summary>
        /// Tries to find a suitable <c>Slot</c> to store the <c>Item</c> in and returns it<br>
        /// Called by the <c>Inserter</c> that is set in the inspector
        /// </summary>
        public Slot InsertionIntoInserter(Inserter inserter, Item i)
        {
            foreach (Target t in targets)
            {
                if (t.itemInfo == i.itemInfo)
                {
                    return t.slot;
                }
            }
            return null;
        }
    }
}