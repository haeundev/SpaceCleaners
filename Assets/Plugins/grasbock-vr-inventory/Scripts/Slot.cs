using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GRASBOCK.XR.Inventory
{
    [RequireComponent(typeof(Collider))]
    public class Slot : MonoBehaviour
    {
        public bool isItemInfoAlreadyAssigned = false; //whether item info is assigned from the beginning

        public float acceleration_multiplier = 1.0f; // the maximum force that will pull in the item when touching the slot
        // references to all items that are to be pulled into the slot
        private Dictionary<Item, (Rigidbody, Collider)> item_gameObjects = new Dictionary<Item, (Rigidbody, Collider)>();

        //send message whenever itemcount is changed to the subscribers
        public delegate void OnItemCountChange(Slot slot);

        public event Action OnSlotCollision;
        public List<OnItemCountChange> item_count_subscribers = new List<OnItemCountChange>();

        // stores the preview
        private Preview preview = null; 
        class Preview
        {
            public GameObject gameObject;
            Collider[] colliders;
            public Preview(Slot slot) {
                //Debug.Log("CreatePreviewGameObject");
                gameObject = Instantiate(slot.ItemInfo.prefab, slot.transform.position, slot.transform.rotation);
                gameObject.name = "ItemPreview";
                // remove all components that are not used for visuals in reverse order.
                // assumes that the order of components is that the dependencies are last!
                // for example there are many components that depend on rigid-bodies. Please make sure they are underneath the rigidbody component.
                Component[] components = gameObject.GetComponents<Component>();
                for (int i = components.Length - 1; i >= 0; i--)
                {
                    var comp = components[i];
                    if (!(comp is MeshRenderer) &&
                        !(comp is MeshFilter) &&
                        !(comp is Collider) &&
                        !(comp is Transform)
                    )
                    {
                        Destroy(comp);
                    }
                }
                // set parent
                gameObject.transform.parent = slot.transform;
                // resize a little
                MeshRenderer mr = gameObject.GetComponentInChildren<MeshRenderer>();
                if (slot.extent == 0.0f) slot.DetermineExtent();

                float scaling_factor = slot.extent / mr.bounds.extents.magnitude * 0.9f;
                if (scaling_factor > 1.0f) scaling_factor = 1.0f; // not bigger than the original
                gameObject.transform.localScale *= scaling_factor;
                // get the colliders and deactivate them
                colliders = gameObject.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    collider.enabled = true;
                    collider.isTrigger = true;  
                }
                // make kinematic
                Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.useGravity = false;
                // preview lets the slot know when a grabber touches it
                ItemPreview ip = gameObject.AddComponent<ItemPreview>();
                ip.on_touch = slot.SpawnGrabbable;

            }

            /// <summary>
            /// enables all colliders in children and the main GameObject to allow the preview to register a grabber<br></br>
            /// or disables them to save computation resources
            /// </summary>
            public void SetCollidersEnabled(bool enabled)
            {
                foreach (Collider collider in colliders)
                    collider.enabled = enabled;
            }
        }

        [SerializeField]
        private ItemInfo m_item_info = null;
        public ItemInfo ItemInfo
        {
            get { return m_item_info; }
            private set {
                m_item_info = value;
            ; }
        }
        //[SerializeField]
        uint m_grabbers = 0;
        uint Grabbers
        {
            get { return m_grabbers; }
            set
            {
                m_grabbers = value;
                if (preview == null) return;
                // collider of the preview is only needed, when grabbers are in the vicinity
                if(value > 0) preview.SetCollidersEnabled(true);
                else preview.SetCollidersEnabled(false);
            }
        }

        [SerializeField]
        private uint m_item_count = 0;
        public uint ItemCount
        {
            get { return m_item_count; }
            set
            {
                m_item_count = value;
                if (value > 0)
                {
                    // show a preview
                    if (preview != null) preview.gameObject.SetActive(true);
                    else preview = new Preview(this);
                }
                else
                {
                    // disable preview
                    if (preview != null)
                    {
                        Destroy(preview.gameObject);
                        preview.gameObject.SetActive(false);
                        preview = null;
                    }

                    if(!isItemInfoAlreadyAssigned)
                    {
                        ItemInfo = null;
                    }

                }
                // notify all subscribers
                foreach(OnItemCountChange func in item_count_subscribers)
                {
                    func(this);
                }
            }
        }

        private float extent = 0.0f; // describes the size of the slot (area of influence)
        /// <summary>
        /// sets the <c>Slot</c> extent
        /// </summary>
        void DetermineExtent()
        {
            Collider collider = GetComponent<Collider>();
            if (!collider)
            {
                Debug.LogError("GameObject " + gameObject.name + " needs to have a collider attached for the Slot to work");
                return;
            }
            if (collider is SphereCollider)
            {
                float factor = transform.localToWorldMatrix.MultiplyVector(Vector3.up).magnitude;
                extent = (collider as SphereCollider).radius * factor;
            }
            else
            {
                Vector3 extents = collider.bounds.extents;
                float[] values = { extents.x, extents.y, extents.z };
                Array.Sort(values); // sort ascending
                extent = values[0];
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            DetermineExtent();
            // set values that have been set in the inspector
            ItemCount = ItemCount;
            ItemInfo = ItemInfo;
        }

        /// <summary>
        /// attracts the Item GameObjects into the slot
        /// </summary>
        void FixedUpdate()
        {
            foreach(KeyValuePair<Item, (Rigidbody, Collider)> entry in item_gameObjects)
            {
                Item item = entry.Key;
                (Rigidbody rb, Collider collider) = entry.Value;
                if (rb == null)
                {
                    Debug.LogError("Rigidbody of attracted item is null. This should never happen. Either ask for support or disable this message if you know what you are doing.");
                    item_gameObjects.Remove(item);
                    continue;
                }
                // calculate attraction force
                Vector3 delta = transform.position - rb.transform.position;
                float sqr_distance = delta.sqrMagnitude;
                Vector3 direction = delta.normalized;
                float relative_size_unit = extent*extent + collider.bounds.extents.sqrMagnitude;
                float x = sqr_distance / relative_size_unit;
                if (x > 0.99f) x = 0.99f; // clamp it down to the [0, 1] range
                Vector3 acceleration = (-4f*(x-0.5f)*(x-0.5f) + 1f) * acceleration_multiplier * direction;

                // counter gravity
                if (rb.useGravity)
                {
                    acceleration -= Physics.gravity;
                }
                Vector3 force = acceleration * rb.mass;

                rb.AddForce(force);

                // emulate drag
                const float drag = 10.0f;
                float multiplier = 1.0f - drag * Time.fixedDeltaTime;
                if (multiplier < 0.0f) multiplier = 0.0f;
                rb.velocity = multiplier * rb.velocity;
            }
        }

        /// <summary>
        /// rotates preview and handles storing of storable items
        /// </summary>
        private void Update()
        {
            if (preview != null)
            {
                preview.gameObject.transform.Rotate(0.0f, 10.0f * Time.deltaTime, 0.0f, Space.World);
            }

            // check each item if it is storable
            List<Item> schedule_store = new List<Item>();
            uint add_count = 0;
            foreach (KeyValuePair<Item, (Rigidbody, Collider)> entry in item_gameObjects)
            {
                Item item = entry.Key;
                (Rigidbody rb, Collider collider) = entry.Value;
                if (item.grabbers.Count > 0) continue;
                // item is not being touched
                //Debug.Log("not grabbed"); 
                if (rb == null)
                {
                    Debug.LogError("Rigidbody of attracted item is null. This should never happen. Either ask for support or disable this message if you know what you are doing.");
                    item_gameObjects.Remove(item);
                    continue;
                }
                // check whether center of slot is inside the item
                Vector3 delta = rb.transform.position - transform.position;
                float sqrDistance = delta.sqrMagnitude;
                if (sqrDistance != 0.0f)
                {
                    rb.velocity = Vector3.Dot(delta, rb.velocity) / sqrDistance * delta; // prevent the item from falling out of the inventory
                    Vector3 surface_point = collider.ClosestPoint(transform.position - 2.0f * collider.bounds.extents.magnitude * delta.normalized);
                    //Debug.DrawLine(transform.position, surface_point, Color.red, 10.0f);
                    if (sqrDistance > (surface_point - rb.transform.position).sqrMagnitude) continue;
                }
                //Debug.Log(item.gameObject.name + " will be stored");
                // close enough to center to be stored
                ItemInfo = item.itemInfo;
                schedule_store.Add(item);
            }
            foreach (Item item in schedule_store)
            {
                item_gameObjects.Remove(item);
                if (item.Slot == null) add_count += 1;
                item.Slot = this; // to make sure it knows that it is stored for subsequent storing calls, because destruction doesn't happen immediately
                Destroy(item.gameObject);
            }
            if(schedule_store.Count > 0) ItemCount += add_count;
        }

        /// <summary>
        /// allows storing items into the slot. Returns the number of items that could be stored. <br>
        /// returns 0 when the Items are not storable
        /// </summary>
        public uint StoreItems(uint count, ItemInfo item_info)
        {
            if (ItemInfo && item_info != ItemInfo) return 0;
            // the item types match or it's free; Can stack
            ItemInfo = item_info;
            ItemCount += count;
            return count;
        }

        /// <summary>
        /// rotates preview and handles storing of storable items
        /// </summary>
        public void Attract(Item item, Collider collider)
        {
            if (m_item_info && item.itemInfo != m_item_info) return;
            //Debug.Log("matching item types");
            // the item types match; Can stack
            if (item.Slot != null && item.Slot != this) return;
            // item doesn't belong to any inventory
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb.isKinematic) return;
            // item is meant to be attracted
            // add it to the list of items that are going to get pulled into the inventory
            //Debug.Log("Attracting " + item.gameObject.name);

            //Debug.Log("Attract");
            // if an item has multiple colliders this can be called multiple times. So allow multiple attempts to add
            item_gameObjects[item] = (rb, collider);

            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;//RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            rb.useGravity = false;

        }

        /// <summary>
        /// tells the slot to stop attracting the specified item
        /// </summary>
        public void StopAttract(Item item)
        {
            //Debug.Log("Stopping to attract");
            item_gameObjects.Remove(item);
        }

        private void OnTriggerEnter(Collider other)
        {
            Grabber grabber = other.GetComponent<Grabber>();
            if (grabber)
            {
                Grabbers += 1;
                return;
            }

            Item item = other.GetComponent<Item>();
            if (item)
            {
                // collider is an item
                //Debug.Log("Item is inside:" + other.gameObject);
                OnSlotCollision?.Invoke();
                Attract(item, other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Grabber grabber = other.gameObject.GetComponent<Grabber>();
            if (grabber)
            {
                Grabbers -= 1;
                return;
            }

            Item item = other.GetComponent<Item>();
            if (item)
            {
                item_gameObjects.Remove(item);

                Rigidbody rb = item.gameObject.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.None;
                rb.useGravity = true;

                if (item.Slot == this)
                {
                    Release(item);
                    
                }
            }
        }

        /// <summary>
        /// releases an <c>Item</c> from the <c>Slot</c>. Decrementing the <c>ItemCount</c>
        /// </summary>
        void Release(Item item)
        {
            item_gameObjects.Remove(item);
            item.Slot = null;
            ItemCount -= 1;
        }

        /// <summary>
        /// Spawn <c>GameObject</c> based on the Prefab specified in the <c>ItemInfo</c>. This is to allow the user to take an <c>Item GameObject</c> from the <c>Slot</c>.
        /// </summary>
        void SpawnGrabbable(Grabber grabber)
        {
            if (grabber.touched_items.Count != 0) return;
            // grabber is not touching anything else
            // spawn an item in here
            GameObject go = Instantiate(ItemInfo.prefab, transform.position, transform.rotation);
            Item item = go.GetComponent<Item>();
            item.Slot = this;
            // it will be stored automatically in OnTriggerEnter
            // make sure there is no preview if there are too many items already pulled out
            uint instantiated_items = 1;
            foreach(Item i in item_gameObjects.Keys)
                if (i.Slot == this) instantiated_items += 1;
            if (ItemCount - instantiated_items == 0)
                // if all instantiated items were to be pulled out, itemcount would be 0, so the preview should vanish
                preview.gameObject.SetActive(false);
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// closes the inventory and stores all items, that belong to the inventory and are not touched by grabbers
        /// </summary>
        public void Close()
        {
            // remove all ungrabbed items, that belong to the slot
            List<Item> schedule_release = new List<Item>(); // because I cannot remove from the same collection i am iterating over
            foreach (Item item in item_gameObjects.Keys)
                if (item.Slot == this)
                {
                    if (item.grabbers.Count == 0) Destroy(item.gameObject);
                    else schedule_release.Add(item); 
                }
            foreach (Item item in schedule_release)
                Release(item); // free it
            // remove tracking list
            item_gameObjects.Clear();
            Grabbers = 0;
            // deactivate
            gameObject.SetActive(false);
            
        }
    }
}

