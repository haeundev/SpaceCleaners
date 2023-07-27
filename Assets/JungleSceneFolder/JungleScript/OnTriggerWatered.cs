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

        public enum Ingredient{Leaf, Flower, Bug, Oxygen};
        public Ingredient spawnIngredient;

        public GameObject[] ingredients;

        bool isIngredientSpawned = false;

        public enum Level{Easy, Hard};
        public Level plantLevel;

        public Animator myAnimator;
        public RuntimeAnimatorController[] animators;

        bool isAnimatorEnabled = false;
        bool isFullyGrown = false;
        string lastAnimationName = "Grow";

        int dropCount = 0;


        void Start()
        {
            if(plantLevel == Level.Easy)
            {
                myAnimator.runtimeAnimatorController = animators[0];
            }
            else if(plantLevel == Level.Hard)
            {
                myAnimator.runtimeAnimatorController = animators[1];
            }
        }

        void Update()
        {
            if(isFullyGrown && !isIngredientSpawned && myAnimator.GetCurrentAnimatorStateInfo(0).IsName(lastAnimationName) && myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0))
            {
                ingredients[(int)spawnIngredient].SetActive(true);
                isIngredientSpawned = true;
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

                if(plantLevel == Level.Easy)
                {
                    lastAnimationName = "Grow";
                    isFullyGrown = true;

                }
                else if(plantLevel == Level.Hard)
                {
                    dropCount++;
                    if(dropCount == 200)
                    {
                        myAnimator.SetTrigger("grow");
                        lastAnimationName = "grow3";
                        isFullyGrown = true;
                        // dropCount = 0;
                    }
                    else if(dropCount == 100)
                    {
                        myAnimator.SetTrigger("grow");
                    }
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
