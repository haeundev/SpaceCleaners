using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GRASBOCK.XR.Inventory
{
    public class InventoryInputHandler : MonoBehaviour
    {
        // this allows to open the inventory at a certain position without being attached to the transform in the hierarchy
        // this is useful if you want it to be placed at the Hand when opening it, but not move with it
        public Transform target;
        public bool copy_orientation = true;
        public Inventory inventory = null;
        private void Start()
        {
            if (!target) target = gameObject.transform.parent;
            if (!inventory) inventory = GetComponent<Inventory>();
        }

        void SetOrientation()
        {
            if (!copy_orientation) return;
            if (!target) return;
            transform.SetPositionAndRotation(target.position, target.rotation);
        }

        /// <summary>
        /// Opens the referenced Inventory when the InputAction has been performed
        /// </summary>
        public void Open(InputAction.CallbackContext context)
        {
            if (!inventory) return;
            if (!context.performed) return;
           
            SetOrientation();
            inventory.Open();
        }

        /// <summary>
        /// Closes the referenced Inventory when the InputAction has been performed
        /// </summary>
        public void Close(InputAction.CallbackContext context)
        {
            if (!inventory) return;
            if (!context.performed) return;
            inventory.Close();
        }
    }
}