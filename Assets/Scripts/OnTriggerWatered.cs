using System;
using UnityEngine.Events;

namespace UnityEngine.XR.Content.Interaction
{
    /// <summary>
    /// Calls functionality when a physics trigger occurs
    /// </summary>
    public class OnTriggerWatered : MonoBehaviour
    {
        [Serializable] public class TriggerEvent : UnityEvent<GameObject> { }

        [SerializeField]
        [Tooltip("If set, this trigger will only fire if the other gameobject has this tag.")]
        string m_RequiredTag = string.Empty;

        [SerializeField]
        [Tooltip("Events to fire when a matcing object collides with this trigger.")]
        TriggerEvent m_OnEnter = new TriggerEvent();

        [SerializeField]
        [Tooltip("Events to fire when a matching object stops colliding with this trigger.")]
        TriggerEvent m_OnExit = new TriggerEvent();

        /// <summary>
        /// If set, this trigger will only fire if the other gameobject has this tag.
        /// </summary>
        public string requiredTag => m_RequiredTag;

        /// <summary>
        /// Events to fire when a matching object collides with this trigger.
        /// </summary>
        public TriggerEvent onEnter => m_OnEnter;

        /// <summary>
        /// Events to fire when a matching object stops colliding with this trigger.
        /// </summary>
        public TriggerEvent onExit => m_OnExit;

        enum Ingredient{Bug, Flower, Leaf, Oxygen};
        Ingredient spawnIngredient;

        public GameObject[] ingredients;

        public Animator myAnimator;
        bool isAnimatorEnabled = false;
        bool isFullyGrown = false;

        int dropCount = 0;

        public GameObject oxygen;

        void Update()
        {
            if(isFullyGrown && myAnimator.GetCurrentAnimatorStateInfo(0).IsName("grow3") && myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0))
            {
                oxygen.SetActive(true);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (CanTrigger(other.gameObject))
                m_OnEnter?.Invoke(other.gameObject);
        }

        void OnTriggerExit(Collider other)
        {
            if (CanTrigger(other.gameObject))
                m_OnExit?.Invoke(other.gameObject);
        }

        void OnParticleCollision(GameObject other)
        {

            if (CanTrigger(other.gameObject) && !isFullyGrown)
            {

                if(!isAnimatorEnabled)
                {
                    m_OnEnter?.Invoke(other);
                    isAnimatorEnabled = true;
                }

                dropCount++;
                if(dropCount == 200)
                {
                    myAnimator.SetTrigger("grow");
                    isFullyGrown = true;
                    // dropCount = 0;
                }
                else if(dropCount == 100)
                {
                    myAnimator.SetTrigger("grow");
                }
                
            }
                
        }

        bool CanTrigger(GameObject otherGameObject)
        {
            if (m_RequiredTag != string.Empty)
                return otherGameObject.CompareTag(m_RequiredTag);
            else
                return true;
        }
    }
}
